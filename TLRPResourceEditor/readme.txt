TLRP Resource Editor 2.2

Licence: MIT
Author:  enceler
Year:    2016

C# 6, .NET Framework 4.5

Libraries used:
  mahapps.metro (http://mahapps.com/) for the metro-style interface (Ms-PL)
  Fody (MIT)
  Fody.PropertyChanged (https://github.com/Fody/PropertyChanged) for auto INotifyPropertyChanged (MIT)

The mahapps.metro library will be embedded in the binary when compiling to create a single-file program. Fody is only used at compile time.
Due to the licence used (GPL2), lzo decompression for assets has been temporarily removed.


Project structure
 ./                             
   MainWindow.xaml                  Entry point for the UI
 ./Data/
        Files.cs                    All filesystem-related operations: read tlr path, look for files, calculate file offsets
        Names.cs                    All string-table related operations: name tables and id-to-name mappings
        UPKFile.cs                  Currently not used; read and process upk/tlr files; extract textures and music
 ./Models/
        Equipment.cs                Table 127 (Equipment)
        Map.cs                      Process map files (spawns, interactions)
        Monster.cs                  Table 259 (Monsters)
        MonsterFormation.cs         Table 123 (Monster Formations)
        Unit.cs                     Table 260 (Friendly Characters) and 270 (Party Talk)
 ./Resources                        Embedded Resources, such as images
 ./ViewModels                       ViewModels used by the Views
 ./Views                            Views

TO DO:
 * Add map image to map lists; show spawn directly on the map
 * Add details for "Flag"s in spawn rules; possibly display rules directly as a tree
 * Add detail window for automatic changes
 * Add ability to change all default values of all tables
 * Finally add string editor
 * Move all file-writing operations to a single location to remove duplicate code

The MIT Licence (MIT)
Copyright (c) 2016 enceler

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
