using System.IO;
using Xunit;

namespace TreeLinq.Tests.Unit
{
    public class Examples
    {
        [Fact]
        public void DirectoryExample()
        {
            var solutionRootFolder = new DirectoryInfo(@"..\..\..\..");
            var solutionFolderTree = Tree<DirectoryInfo>.Grow(
                solutionRootFolder, 
                f => f, 
                f => f.GetDirectories());

            var unitTestFoldersCount = solutionFolderTree.CountByValue(f => f.Name.EndsWith(".Unit"));

            Assert.Equal(1, unitTestFoldersCount);
        }
    }
}
