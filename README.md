This project contains code that will do Diff generation, comparing
two collections of any type and outputting the differences between
the two, including an attempt to align up differences, specifically
in text files.

The library also comes with 3-way merge functionality, allowing you
to take two collections that both started as a 3rd common base but
has been modified, and then merge the modifications into a final
output. This is the type of 3-way merge that version control systems
do.

Details:

* Language: C# 13.0
* Runtime:
  * .NET Framework 4.5
  * .NET Framework 4.6.1
  * .NET Standard 1.0
  * .NET Standard 2.0
  * .NET Standard 2.1
  * .NET 5.0
  * .NET 6.0

Repository and project location: [GitHub][1]  
Maintainer: [Lasse V. Karlsen][2], [Ross King][3]

---

# GPG Signing

If you want to import my GPG key to verify my commits you can do it
using one of these commands:

    git cat-file blob pubkey | gpg --import
    git cat-file blob pubkey | gpg2 --import

  [1]: https://github.com/altavec/DiffLib
  [2]: mailto:lasse@vkarlsen.no
  [3]: mailto:ross.king@altavec.com
