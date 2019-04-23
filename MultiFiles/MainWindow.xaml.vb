Imports System
Imports System.Windows
Imports Microsoft.Win32
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Threading
Imports System.IO
Imports System.ComponentModel
Imports System.Windows.Data
Imports System.Globalization
Imports System.Linq

Class ReadOnlyObservableCollectionPerso(Of T)
    Inherits ReadOnlyObservableCollection(Of T)

    Public Sub New(list As ObservableCollection(Of T))
        MyBase.New(list)
    End Sub

    Public Property GroupStatus As Boolean
        Get
            If (Me.Count <= 1) Then
                Return False
            Else
                Dim b As Boolean = True
                Dim sfi As SearchFileInfo
                For Each i As Object In Me
                    If TypeOf i Is SearchFileInfo Then
                        sfi = CType(i, SearchFileInfo)
                        b = b And sfi.ToBeDel
                    End If
                Next
                Return b
            End If
        End Get
        Set(value As Boolean)
            Dim sfi As SearchFileInfo
            For Each i As Object In Me
                If TypeOf i Is SearchFileInfo Then
                    sfi = CType(i, SearchFileInfo)
                    sfi.ToBeDel = value
                End If
            Next
        End Set
    End Property
End Class

Class ListCollectionViewPerso
    Inherits ListCollectionView

    Sub New(list As IList)
        MyBase.New(list)
    End Sub

    Overloads ReadOnly Property Groups As ReadOnlyObservableCollectionPerso(Of Object)

End Class

Public Class GroupSizeToExpanderConverter
    Implements IValueConverter

    Private Function IValueConverter_Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim grp As CollectionViewGroup
        grp = CType(value, CollectionViewGroup)
        Return grp.Items.Count() > 1
    End Function

    Private Function IValueConverter_ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class

Class MainWindow

    Dim listSearchDir As New ObservableCollection(Of SearchDirectory)
    Dim listSearchFile As New ObservableCollection(Of SearchFileInfo)
    Public Property GroupedCustomers As ICollectionView
    Dim tasks As New List(Of Task)()
    Private messagesLock As New Object

    Delegate Sub dUpdateList(rowToAdd As SearchFileInfo)
    Delegate Sub dUpdateDisplay()

    Public Sub New()
        InitializeComponent()
        lvDirSearch.ItemsSource = listSearchDir

        GroupedCustomers = New ListCollectionViewPerso(listSearchFile)
        GroupedCustomers.GroupDescriptions.Add(New PropertyGroupDescription("CheckSum"))


        datagrid.ItemsSource = GroupedCustomers

    End Sub

    Private Sub AnalyseGroup(gr As System.Windows.Data.ListCollectionView)
        For Each g In gr.Groups
            For Each i As SearchFileInfo In g.Items

            Next
        Next
    End Sub

    Private Sub Btn_Click_Rem_Dir(sender As Object, e As RoutedEventArgs)
        Dim selItem = lvDirSearch.SelectedItems(0)
        If Not selItem Is Nothing Then
            listSearchDir.Remove(selItem)
            lvDirSearch.UpdateLayout()
            lvDirSearch.Items.Remove(selItem)
            If lvDirSearch.Items.Count <= 0 Then
                btRemove.IsEnabled = False
            End If
        End If
    End Sub

    Private Sub Btn_Click_Add_Dir(sender As Object, e As RoutedEventArgs)
        Dim openFileDialog1 As New System.Windows.Forms.FolderBrowserDialog()
        Dim result As System.Windows.Forms.DialogResult = openFileDialog1.ShowDialog()

        If (result = System.Windows.Forms.DialogResult.OK) Then
            Dim s As String
            s = openFileDialog1.SelectedPath
            Dim searchDir = New SearchDirectory()
            searchDir.DirectoryPath = s
            searchDir.ToBeDel = False

            listSearchDir.Add(searchDir)
            lvDirSearch.UpdateLayout()
            If lvDirSearch.Items.Count > 0 Then
                btRemove.IsEnabled = True
            End If

        ElseIf (result = System.Windows.Forms.DialogResult.Cancel) Then
                Return
        End If
    End Sub

    Private Sub Btn_Click_Launch(sender As Object, e As RoutedEventArgs)
        Dim threadAnalysis = New Thread(AddressOf LaunchAnalysis)
        threadAnalysis.Start(listSearchDir)

        'tasks.Add(Task.Run(Sub() LaunchAnalysis(listSearchDir)))
        'LaunchAnalysis(listSearchDir)
        Task.WaitAll(tasks.ToArray())

        'AnalyseGroup(Me.datagrid.ItemsSource)
        threadAnalysis.Join()
        'CheckGroups()

    End Sub

    Private Sub Btn_Click_Cancel(sender As Object, e As RoutedEventArgs)

    End Sub




    ''' <summary>
    ''' Main function of the software. It will run accross the directory list and identify all the files.
    ''' In order to increase the recognition of the similar items, we open PDF file in order to recover some data
    ''' and we compute a SHA512 key.
    ''' Results will be displayed in a DataGridView thanks a DisplayInformation Object.
    ''' </summary>
    ''' <param name="l">List of Directories.</param>
    Public Sub LaunchAnalysis(l As ObservableCollection(Of SearchDirectory))
        For Each s In l
            AnalyzeSelectedDir(s)
        Next

    End Sub

    Public Sub CheckGroups()
        Dim cvg As Object
        Dim sfi As SearchFileInfo
        For Each cvg In GroupedCustomers.Groups
            If cvg.ItemCount = 1 Then 'if onbly one item, uncheck ze keep at least one
                sfi = TryCast(cvg.Items(1), SearchFileInfo)
                If Not IsNothing(sfi) Then
                    sfi.ToBeDel = False
                End If
            ElseIf cvg.ItemCount > 1 Then 'if there several items, ensures that at least one remain
                Dim b As Boolean
                b = False
                For Each sfi In cvg.Items
                    If Not sfi.ToBeDel Then
                        b = True
                        Exit For
                    End If
                Next

                If b = False Then
                    sfi = TryCast(cvg.Items(0), SearchFileInfo)
                    sfi.ToBeDel = False
                End If

            End If
        Next

    End Sub


    Private Sub AnalyzeSelectedDir(s As SearchDirectory)

        Dim listDir As String()
        Dim sfi As SearchFileInfo
        If Not s.DirectoryPath Is Nothing Then
            'tasks.Add(Task.Factory.StartNew(Sub() ListFileInPath(s.DirectoryPath, s)))
            tasks.Add(Task.Run(Sub() ListFileInPath(s.DirectoryPath, s)))
            listDir = Directory.GetDirectories(s.DirectoryPath, "*.*", SearchOption.AllDirectories)

            For Each d In listDir
                'tasks.Add(Task.Factory.StartNew(Sub() ListFileInPath(d, s)))
                tasks.Add(Task.Run(Sub() ListFileInPath(d, s)))
            Next

        End If

    End Sub

    Private Sub ListFileInPath(pth As String, s As SearchDirectory)
        Dim listFile As String()
        Try
            listFile = Directory.GetFiles(pth, "*.pdf")
            For Each fileInDir In listFile
                'AnalyseDir(fileInDir, s)
                Dim fInfo As SearchFileInfo
                fInfo = New SearchFileInfo()
                Debug.WriteLine(fileInDir)
                fInfo.FullPath = fileInDir
                fInfo.ToBeDel = s.ToBeDel
                Application.Current.Dispatcher.Invoke(New dUpdateList(AddressOf UpdateList), fInfo)
            Next
            Application.Current.Dispatcher.Invoke(New dUpdateDisplay(AddressOf UpdateDisplay))
        Catch ex As System.UnauthorizedAccessException
            Console.WriteLine(ex.Message)
        End Try
    End Sub


    Private Function AnalyseDir(fileInDir As String, s As SearchDirectory)
        Dim fInfo As SearchFileInfo
        fInfo = New SearchFileInfo()
        fInfo.FullPath = fileInDir
        fInfo.ToBeDel = s.ToBeDel
        Application.Current.Dispatcher.Invoke(New dUpdateDisplay(AddressOf UpdateDisplay), fInfo)
    End Function

    Private Sub UpdateDisplay()

        datagrid.UpdateLayout()

    End Sub

    Private Sub UpdateList(sf As SearchFileInfo)
        SyncLock messagesLock
            listSearchFile.Add(sf)
            datagrid.UpdateLayout()
        End SyncLock


    End Sub

    Private Sub DelSelectInGroup_Click(sender As Object, e As RoutedEventArgs)

    End Sub
End Class
