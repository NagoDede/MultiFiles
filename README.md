# MultiFiles
##Aim of the project
MultiFiles is a quick and dirty tool build to find and delete similar Files (especially PDF files).
The origin of the tool comes after a hard disk crash; I recover a large amount of data but hundred (thousand) of my pdf files where copy of each other.
During the recovery process, the file name is not recovered or partially recovered. Even the file extension could be unrecovered.

Multifiles aims to navigate though several directory, find the files and compute a check sum. This checksum will be used to identify similar files.
Multifiles will then group these files and support the deletion of these files (not yet implemented).

##Langage choice
I elect to do this with VBA because I don't want to spare my time on different things. Thanks Visual Basic, it is easy to create a form, the syntax is simple (may be not the most efficient) and it supports well all the Windows libraries.
Of course, it is possible to do the same in Python, Lua, C++, C, C#, Go... but a choice should be done.

##
Note: There is no Licence associated (not yet). Consider this a GNU licence at the time being.
