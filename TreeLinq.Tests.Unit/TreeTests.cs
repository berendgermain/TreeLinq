using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TreeLinq.Tests.Unit
{
    public class TreeTests
    {
        private readonly Tree<Species> _root = Tree(new Species("root"));

        [Fact]
        public void GivenTwoIdenticalTreesChildren_Equals_ReturnsTrue()
        {
            var tree1 = Tree(new Species("A"));
            var tree2 = Tree(new Species("A"));

            Assert.True(tree1.Equals(tree2));
        }

        [Fact]
        public void GivenTwoDifferentTrees_Equals_ReturnsFalse()
        {
            var tree1 = Tree(new Species("A"));
            var tree2 = Tree(new Species("B"));

            Assert.False(tree1.Equals(tree2));
        }

        [Fact]
        public void GivenTwoIdenticalTreesWithIdenticalChildren_Equals_ReturnsTrue()
        {
            var tree1 = Tree(new Species("A"), new Species("B"));
            var tree2 = Tree(new Species("A"), new Species("B"));

            Assert.True(tree1.Equals(tree2));
        }

        [Fact]
        public void GivenTwoIdenticalTreesWithDifferentChildren_Equals_ReturnsFalse()
        {
            var tree1 = Tree(new Species("A"), new Species("B"));
            var tree2 = Tree(new Species("A"), new Species("C"));

            Assert.False(tree1.Equals(tree2));
        }

        [Fact]
        public void GivenRootItem_Root_ReturnsItself()
        {
            Assert.Equal(_root.Root, _root);
        }

        [Fact]
        public void GivenRootItem_IsRoot_ReturnsTrue()
        {
            Assert.True(Tree(new Species()).IsRoot);
        }

        [Fact]
        public void GivenChildItem_IsRoot_ReturnsFalse()
        {
            var subspecies = _root.AddChildByValue(new Species());

            Assert.False(subspecies.IsRoot);
        }

        [Fact]
        public void GivenRootItem_Parent_ReturnsNull()
        {
            Assert.Null(Tree(new Species()).Parent);
        }

        [Fact]
        public void GivenChildItem_Parent_ReturnsParent()
        {
            var subspecies = _root.AddChildByValue(new Species());

            Assert.Equal(_root, subspecies.Parent);
        }

        [Fact]
        public void GivenRootItemWithoutChildren_IsLeaf_ReturnsTrue()
        {
            Assert.True(_root.IsLeaf);
        }

        [Fact]
        public void GivenRootItemWithChild_IsLeaf_ReturnsFalse()
        {
            _root.AddChildByValue(new Species());

            Assert.False(_root.IsLeaf);
        }

        [Fact]
        public void GivenRootItemWithoutChildren_Children_ReturnsEmptyList()
        {
            Assert.Empty(_root.Children);
        }

        [Fact]
        public void GivenRootItemWithChild_Children_ReturnsListWithChild()
        {
            _root.AddChildByValue(new Species());

            Assert.Single(_root.Children);
        }

        [Fact]
        public void GivenRootItemWithoutChildren_ChildrenValues_ReturnsEmptyList()
        {
            Assert.Empty(_root.ChildrenValues);
        }

        [Fact]
        public void GivenRootItemWithChild_ChildrenValues_ReturnsListWithChild()
        {
            _root.AddChildByValue(new Species());

            Assert.Single(_root.ChildrenValues);
        }

        [Fact]
        public void GivenTreeWithoutChildren_HasChildren_ReturnsFalse()
        {
            Assert.False(_root.HasChildren);
        }

        [Fact]
        public void GivenTreeWithChildren_HasChildren_ReturnsTrue()
        {
            _root.AddChildByValue(new Species());
            
            Assert.True(_root.HasChildren);
        }

        [Fact]
        public void GivenARootTree_IsFirstChild_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _root.IsFirstChild);
        }

        [Fact]
        public void GivenATreeWithSeveralChildrenAndFirstChild_IsFirstChild_ReturnsTrue()
        {
            var firstChild = Tree(new Species("A"));
            _root.AddChildren(firstChild, Tree(new Species("B")));

            Assert.True(firstChild.IsFirstChild);
        }

        [Fact]
        public void GivenATreeWithSeveralChildrenAndLastChild_IsFirstChild_ReturnsFalse()
        {
            var secondChild = Tree(new Species("B"));
            _root.AddChildren(Tree(new Species("A")), secondChild);

            Assert.False(secondChild.IsFirstChild);
        }

        [Fact]
        public void GivenARootTree_IsLastChild_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _root.IsFirstChild);
        }

        [Fact]
        public void GivenATreeWithSeveralChildrenAndFirstChild_IsLastChild_ReturnsFalse()
        {
            var firstChild = Tree(new Species("A"));
            _root.AddChildren(firstChild, Tree(new Species("B")));

            Assert.False(firstChild.IsLastChild);
        }

        [Fact]
        public void GivenATreeWithSeveralChildrenAndLastChild_IsLastChild_ReturnsTrue()
        {
            var secondChild = Tree(new Species("B"));
            _root.AddChildren(Tree(new Species("A")), secondChild);

            Assert.True(secondChild.IsLastChild);
        }

        [Fact]
        public void GivenARootTree_Siblings_ReturnsEmptyList()
        {
            Assert.Equal(Enumerable.Empty<Tree<Species>>(), _root.Siblings);
        }

        [Fact]
        public void GivenATreeWithoutSiblings_Siblings_ReturnsEmptyList()
        {
            var onlyChild = Tree(new Species("A"));
            _root.AddChild(onlyChild);

            Assert.Empty(onlyChild.Siblings);
        }

        [Fact]
        public void GivenATreeWithSiblings_Siblings_ReturnsListWithOtherChildren()
        {
            var firstChild = Tree(new Species("A"));
            var secondChild = Tree(new Species("B"));
            var thirdChild = Tree(new Species("C"));
            _root.AddChildren(firstChild, secondChild, thirdChild);

            var result = secondChild.Siblings;

            Assert.Equal(new[] { firstChild, thirdChild }, result);
        }

        [Fact]
        public void GivenOnlyChildTree_HasSiblings_ReturnsFalse()
        {
            var onlyChild = Tree(new Species("A"));
            _root.AddChild(onlyChild);

            Assert.False(onlyChild.HasSiblings);
        }

        [Fact]
        public void GivenTreeWithSiblings_HasSiblings_ReturnsTrue()
        {
            var child = Tree(new Species("A"));
            _root.AddChildren(child, Tree(new Species("B")));
            
            Assert.True(child.HasSiblings);
        }

        [Fact]
        public void GivenATreeWithSiblings_NextSibling_ReturnsFollowingSibling()
        {
            var firstChild = Tree(new Species("A"));
            var secondChild = Tree(new Species("B"));
            var thirdChild = Tree(new Species("C"));
            _root.AddChildren(firstChild, secondChild, thirdChild);

            var result = secondChild.NextSibling;

            Assert.Equal(thirdChild, result);
        }

        [Fact]
        public void GivenATreeWithSiblings_PreviousSibling_ReturnsPreviousSibling()
        {
            var firstChild = Tree(new Species("A"));
            var secondChild = Tree(new Species("B"));
            var thirdChild = Tree(new Species("C"));
            _root.AddChildren(firstChild, secondChild, thirdChild);

            var result = secondChild.PreviousSibling;

            Assert.Equal(firstChild, result);
        }

        [Fact]
        public void GivenARootTree_GenerationDepth_ReturnsZero()
        {
            Assert.Equal(0, _root.GenerationDepth);
        }

        [Fact]
        public void GivenAGrandChildTree_GenerationDepth_ReturnsThree()
        {
            var greatGrandChild = Tree(new Species("C"), new Species("D"));
            _root.AddChild(
                new Tree<Species>(new Species("A"), 
                    new Tree<Species>(new Species("B"), 
                        greatGrandChild)));
            
            Assert.Equal(3, greatGrandChild.GenerationDepth);
        }

        [Fact]
        public void GivenADeepTree_GenerationCount_ReturnsCorrectCount()
        {
            var species = Tree(
                new Species(
                    "root",
                    new Species(
                        "A",
                        new Species(
                            "B",
                            new Species(
                                "C")))));
            
            Assert.Equal(4, species.GenerationCount);
        }

        [Fact]
        public void GivenADeepTree_Generations_ReturnsListOfGenerations()
        {
            var greatGreatGrandChild1 = Tree(new Species("D"));
            var greatGreatGrandChild2 = Tree(new Species("E"));
            var greatGrandChild = new Tree<Species>(
                new Species("C"), 
                    greatGreatGrandChild1, greatGreatGrandChild2);
            var grandChild = new Tree<Species>(
                new Species("B"), 
                    greatGrandChild);
            var child = new Tree<Species>(
                new Species("A"), 
                    grandChild);
            _root.AddChild(child);

            var result = _root.Generations;
            
            Assert.Equal(
                new[]
                {
                    new[] { _root },
                    new[] { child },
                    new[] { grandChild },
                    new[] { greatGrandChild }, 
                    new[] { greatGreatGrandChild1, greatGreatGrandChild2 }
                },
                result);
        }

        [Fact]
        public void GivenARootItemWithoutChildren_Path_ReturnsRootItemOnly()
        {
            Assert.Single(new Tree<Species>().Path);
        }

        [Fact]
        public void GivenRootItemWithoutChildren_PathValues_ReturnsRootItemValueOnly()
        {
            Assert.Single(new Tree<Species>().PathValues);
        }

        [Fact]
        public void GivenLeafTree_PathValues_ReturnsPathOfDescendant()
        {
            var leaf = Tree(new Species("leaf"));
            _root.AddChildren(
                new Tree<Species>(new Species("A")),
                new Tree<Species>(new Species("B"), 
                    leaf));
            
            Assert.Equal(
                leaf.PathValues, 
                new [] { new Species("root"), new Species("B"), new Species("leaf") });
        }

        [Fact]
        public void GivenRootItem_AddChild_ResultsInRootItemWithChild()
        {
            _root.AddChild(Tree(new Species()));

            Assert.Single(_root.Children);
        }

        [Fact]
        public void GivenRootItem_AddChildByValue_ResultsInRootItemWithChild()
        {
            _root.AddChildByValue(new Species());

            Assert.Single(_root.Children);
        }

        [Fact]
        public void GivenRootItem_AddChildren_ResultsInRootItemWithTwoChildren()
        {
            _root.AddChildrenByValues(new Species(), new Species());

            Assert.Equal(2, _root.Children.Count());
        }

        [Fact]
        public void GivenRootItem_AddChildrenByValues_ResultsInRootItemWithTwoChildren()
        {
            _root.AddChildrenByValues(new Species(), new Species());

            Assert.Equal(2, _root.Children.Count());
        }

        [Fact]
        public void GivenARootTree_RemoveFromParent_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _root.RemoveFromParent());
        }

        [Fact]
        public void GivenATreeWithChildren_RemoveFromParent_RemovesChildFromParentsChildrenList()
        {
            var child = _root.AddChildByValue(new Species("A"));

            child.RemoveFromParent();

            Assert.Empty(_root.Children);
        }

        [Fact]
        public void GivenATreeWithChildren_RemoveChild_RemovesChildFromParentsChildrenList()
        {
            var child = _root.AddChildByValue(new Species("A"));

            _root.RemoveChild(child);

            Assert.Empty(_root.Children);
        }

        [Fact]
        public void GivenATreeWithChildren_RemoveChildren_RemovesChildrenFromParentsChildrenList()
        {
            var child1 = _root.AddChildByValue(new Species("A"));
            var child2 = _root.AddChildByValue(new Species("B"));

            _root.RemoveChildren(child1, child2);

            Assert.Empty(_root.Children);
        }

        [Fact]
        public void GivenANonRootTrees_RemoveChildrenWithSelector_RemovesChildrenFromParentsChildrenList()
        {
            _root.AddChildByValue(new Species("A"));
            _root.AddChildByValue(new Species("B"));

            _root.RemoveChildren<Tree<Species>>(t => t.Value.Name == "A");

            Assert.Single(_root.Children);
            Assert.Equal("B", _root.Children.Single().Value.Name);
        }

        [Fact]
        public void GivenATreeWithChildren_RemoveAllChildren_LeavesTreeChildless()
        {
            _root.AddChildByValue(new Species("A"));
            _root.AddChildByValue(new Species("B"));

            _root.RemoveAllChildren();

            Assert.Empty(_root.Children);
        }

        [Fact]
        public void GivenARootTreeBeingChild_IsChildOf_ReturnsFalse()
        {
            var tree = Tree(new Species("A"));
            Assert.False(_root.IsChildOf(tree));
        }

        [Fact]
        public void GivenATreeBeingChild_IsChildOf_ReturnsTrue()
        {
            var tree = Tree(new Species("A"));
            _root.AddChild(tree);

            Assert.True(tree.IsChildOf(_root));
        }

        [Fact]
        public void GivenTwoSiblingTrees_IsSiblingOf_ReturnsTrue()
        {
            var tree1 = Tree(new Species("A"));
            var tree2 = Tree(new Species("B"));

            _root.AddChildren(tree1, tree2);

            Assert.True(tree1.IsSiblingOf(tree2));
        }

        [Fact]
        public void GivenATreeNotBeingChild_IsChildOf_ReturnsFalse()
        {
            var tree = Tree(new Species("A"));

            Assert.False(tree.IsChildOf(_root));
        }

        [Fact]
        public void GivenATreeBeingParent_IsParentOf_ReturnsTrue()
        {
            var tree = Tree(new Species("A"));
            _root.AddChild(tree);

            Assert.True(_root.IsParentOf(tree));
        }

        [Fact]
        public void GivenATreeNotBeingParent_IsParentOf_ReturnsFalse()
        {
            var tree = Tree(new Species("A"));

            Assert.False(_root.IsParentOf(tree));
        }

        [Fact]
        public void GivenAnHierarchicalStructure_Grow_ReturnsTree()
        {
            var species = new Species(
                "A",
                new Species(
                    "AA",
                    new Species("AAA")),
                new Species(
                    "BB",
                    new Species("BBA"),
                    new Species("BBB")));

            var result = Tree<Species>.Grow(species, t => t.Subspecies);

            Assert.IsType<Tree<Species>>(result);
            Assert.Equal("A", result.Root.Value.Name);
            Assert.Equal("AA", result.Root.Children.First().Value.Name);
            Assert.Equal("AAA", result.Root.Children.First().Children.First().Value.Name);
            Assert.Equal("BB", result.Root.Children.Last().Value.Name);
            Assert.Equal("BBA", result.Root.Children.Last().Children.First().Value.Name);
            Assert.Equal("BBB", result.Root.Children.Last().Children.Last().Value.Name);
        }

        [Fact]
        public void GivenTreeWithItems_TraverseDepthFirstDeep_PerformsActionOnAllTreeItems()
        {
            _root.AddChildren(
                Tree(new Species("A"), new Species("B")), 
                Tree(new Species("C")));

            var result = string.Empty;
            _root.TraverseDepthFirst(t => result += t.Value.Name);

            Assert.Equal("rootABC", result);
        }

        [Fact]
        public void GivenTreeWithItems_TraverseDepthFirstShallow_PerformsActionOnNonSkippedTreeItems()
        {
            var leaf = Tree(new Species("leaf"));
            _root.AddChildren(
                new Tree<Species>(new Species("A")),
                new Tree<Species>(new Species("B"),
                    leaf));

            var result = string.Empty;
            _root.TraverseDepthFirst(t => result += t.Value.Name, t => t.Value.Name == "B");

            Assert.Equal("rootA", result);
        }

        [Fact]
        public void GivenTreeWithItems_TraverseBreadthFirstDeep_PerformsActionOnAllTreeItems()
        {
            _root.AddChildren(
                new Tree<Species>(
                    new Species("A"),
                        new Tree<Species>(new Species("C"), 
                            new Species("E"))), 
                new Tree<Species>(
                    new Species("B"), 
                        new Species("D")));

            var result = string.Empty;
            _root.TraverseBreadthFirst(t => result += t.Value.Name);

            Assert.Equal("rootABCDE", result);
        }

        [Fact]
        public void GivenTreeWithItems_TraverseBreadthFirstWithSkip_PerformsActionOnNonSkippedTreeItems()
        {
            _root.AddChildren(
                new Tree<Species>(
                    new Species("A"),
                    new Tree<Species>(new Species("C"), 
                        new Species("E"))), 
                new Tree<Species>(
                    new Species("B"), 
                    new Species("D")));

            var result = string.Empty;
            _root.TraverseBreadthFirst(t => result += t.Value.Name, t => t.Value.Name == "A");

            Assert.Equal("rootBCDE", result);
        }

        [Fact]
        public void GivenTreeWithSpecifiedItem_Contains_ReturnsTrue()
        {
            _root.AddChildren(
                Tree(new Species("A"), new Species("AA")), 
                Tree(new Species("B")));

            Assert.True(_root.Contains(Tree(new Species("A"), new Species("AA"))));
        }

        [Fact]
        public void GivenTreeWithoutSpecifiedItem_Contains_ReturnsFalse()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.False(_root.Contains(Tree(new Species("C"))));
        }

        [Fact]
        public void GivenTreeWithSpecifiedItem_ContainsByValue_ReturnsTrue()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.True(_root.ContainsByValue(new Species("A")));
        }

        [Fact]
        public void GivenTreeWithoutSpecifiedItem_ContainsByValue_ReturnsFalse()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.False(_root.ContainsByValue(new Species("C")));
        }

        [Fact]
        public void GivenATreeThreeLevelsDeep_ToListDepthFirst_ReturnsListWithItemsInRightOrder()
        {
            var grandChild = Tree(new Species("AA"));
            var child1 = Tree(new Species("A"));
            child1.AddChildren(grandChild);
            var child2 = Tree(new Species("B"));
            _root.AddChildren(child1, child2);

            Assert.Equal(
                new List<Tree<Species>>
                {
                    _root,
                    child1,
                    grandChild,
                    child2
                },
                _root.ToListDepthFirst());
        }

        [Fact]
        public void GivenTreeWithItems_ToListValuesDepthFirst_ReturnsListWithItems()
        {
            var leaf = Tree(new Species("leaf"));
            _root.AddChildren(
                new Tree<Species>(new Species("A"), 
                    leaf),
                new Tree<Species>(new Species("B")));

            var result = _root.ToListValuesDepthFirst();

            Assert.Equal(
                new List<Species>
                {
                    _root.Value,
                    new Species("A"),
                    new Species("leaf"),
                    new Species("B")
                },
                result);
        }

        [Fact]
        public void GivenATreeThreeLevelsDeep_ToListValuesDepthFirst_ReturnsListWithItemsInRightOrder()
        {
            var grandChild = Tree(new Species("AA"));
            var child1 = Tree(new Species("A"));
            child1.AddChildren(grandChild);
            var child2 = Tree(new Species("B"));
            _root.AddChildren(child1, child2);

            Assert.Equal(
                new List<Species>
                {
                    _root.Value,
                    child1.Value,
                    grandChild.Value,
                    child2.Value
                },
                _root.ToListValuesDepthFirst());
        }

        [Fact]
        public void GivenATreeThreeLevelsDeep_ToListBreadthFirst_ReturnsListWithItemsInRightOrder()
        {
            var grandChild = Tree(new Species("AA"));
            var child1 = Tree(new Species("A"));
            child1.AddChildren(grandChild);
            var child2 = Tree(new Species("B"));
            _root.AddChildren(child1, child2);

            Assert.Equal(
                new List<Tree<Species>>
                {
                    _root,
                    child1,
                    child2,
                    grandChild
                },
                _root.ToListBreadthFirst());
        }

        [Fact]
        public void GivenTreeWithItems_ToListValuesBreadthFirst_ReturnsListWithItems()
        {
            var leaf = Tree(new Species("leaf"));
            _root.AddChildren(
                new Tree<Species>(new Species("A"), 
                    leaf),
                new Tree<Species>(new Species("B")));

            var result = _root.ToListValuesBreadthFirst();

            Assert.Equal(
                new List<Species>
                {
                    _root.Value,
                    new Species("A"),
                    new Species("B"),
                    new Species("leaf")
                },
                result);
        }

        [Fact]
        public void GivenATreeThreeLevelsDeep_ToListValuesBreadthFirst_ReturnsListWithItemsInRightOrder()
        {
            var grandChild = Tree(new Species("AA"));
            var child1 = Tree(new Species("A"));
            child1.AddChildren(grandChild);
            var child2 = Tree(new Species("B"));
            _root.AddChildren(child1, child2);

            Assert.Equal(
                new List<Species>
                {
                    _root.Value,
                    child1.Value,
                    child2.Value,
                    grandChild.Value
                },
                _root.ToListValuesBreadthFirst());
        }

        [Fact]
        public void GivenARootTree_GetGeneration_ReturnsListWithRootTreeOnly()
        {
            Assert.Equal(new[] { _root }, _root.GetGeneration(0));
        }

        [Fact]
        public void GivenATreeWithMultipleChildrenAndGrandChildren_GetGeneration_ReturnsAllGrandChildren()
        {
            var grandChild1 = Tree(new Species("AA"));
            var grandChild2 = Tree(new Species("AB"));
            var grandChild3 = Tree(new Species("BA"));
            _root.AddChildren(
                new Tree<Species>(new Species("A"), 
                    grandChild1, grandChild2),
                new Tree<Species>(new Species("B"), 
                    grandChild3));

            var result = _root.GetGeneration(2);

            Assert.Equal(new[] { grandChild1, grandChild2, grandChild3 }, result);
        }

        [Fact]
        public void GivenATreeWithMultipleChildrenAndGrandChildren_GetByChildIndexesPath_ReturnsCorrectTree()
        {
            var grandChild1 = Tree(new Species("AA"));
            var grandChild2 = Tree(new Species("AB"));
            var grandChild3 = Tree(new Species("BA"));
            _root.AddChildren(
                new Tree<Species>(new Species("A"), 
                    grandChild1, grandChild2),
                new Tree<Species>(new Species("B"), 
                    grandChild3));

            var result = _root.GetByChildIndexesPath(0, 1);

            Assert.Equal(grandChild2, result);
        }

        [Fact]
        public void GivenANonExistingChildIndex_GetByChildIndexesPath_ThrowsIndexOutOfRangeException()
        {
            var grandChild1 = Tree(new Species("AA"));
            var grandChild2 = Tree(new Species("AB"));
            var grandChild3 = Tree(new Species("BA"));
            _root.AddChildren(
                new Tree<Species>(new Species("A"), 
                    grandChild1, grandChild2),
                new Tree<Species>(new Species("B"), 
                    grandChild3));

            Assert.Throws<IndexOutOfRangeException>(() => _root.GetByChildIndexesPath(0, 5));
        }

        [Fact]
        public void GivenATreeWithMultipleChildrenAndGrandChildren_GetByGenerationCoordinates_ReturnsCorrectTree()
        {
            var grandChild1 = Tree(new Species("AA"));
            var grandChild2 = Tree(new Species("AB"));
            var grandChild3 = Tree(new Species("BA"));
            _root.AddChildren(
                new Tree<Species>(new Species("A"), 
                    grandChild1, grandChild2),
                new Tree<Species>(new Species("B"), 
                    grandChild3));

            var result = _root.GetByGenerationCoordinates(2, 2);

            Assert.Equal(grandChild3, result);
        }

        [Fact]
        public void GivenANonExistingGenerationDepth_GetByGenerationCoordinates_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _root.GetByGenerationCoordinates(1, 0));
        }

        [Fact]
        public void GivenANonExistingGenerationIndex_GetByGenerationCoordinates_ThrowsIndexOutOfRangeException()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.Throws<IndexOutOfRangeException>(() => _root.GetByGenerationCoordinates(1, 5));
        }

        [Fact]
        public void GivenTreeWithItems_Select_ReturnsTreeWithSelectedItems()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            var result = _root.Select(t => t.Value.Name);

            Assert.IsType<Tree<string>>(result);
            Assert.Equal(new Tree<string>("root", "A", "B"), result);
        }

        [Fact]
        public void GivenTreeWithItems_SelectByValue_ReturnsTreeWithSelectedItems()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            var result = _root.SelectByValue(s => s.Name);

            Assert.IsType<Tree<string>>(result);
            Assert.Equal(new Tree<string>("root", "A", "B"), result);
        }

        [Fact]
        public void GivenTreeWithItems_SelectWithChildrenOrdered_ReturnsTreeWithSelectedItemsInRightOrder()
        {
            _root.AddChildrenByValues(new Species("B"), new Species("A"));

            var result = _root.Select(
                t => t.Value.Name,
                children => children.OrderBy(c => c.Value.Name));

            Assert.IsType<Tree<string>>(result);
            Assert.Equal(new Tree<string>("root", "A", "B"), result);
        }

        [Fact]
        public void GivenTreeWithItems_SelectByValueWithChildrenOrdered_ReturnsTreeWithSelectedItemsInRightOrder()
        {
            _root.AddChildrenByValues(new Species("B"), new Species("A"));

            var result = _root.SelectByValue(
                s => s.Name,
                children => children.OrderBy(c => c.Value.Name));

            Assert.IsType<Tree<string>>(result);
            Assert.Equal(new Tree<string>("root", "A", "B"), result);
        }

        [Fact]
        public void GivenTreeWithTreeItems_Where_ReturnsListWithOnlyDesiredItems()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"), new Species("C"));

            var result = _root.Where(t => t.Value.Name == "C");

            Assert.Equal("C", result.Single().Value.Name);
        }

        [Fact]
        public void GivenTreeWithTreeItems_WhereByValue_ReturnsListWithOnlyDesiredItems()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"), new Species("C"));

            var result = _root.WhereByValue(t => t.Name == "C");

            Assert.Equal("C", result.Single().Name);
        }

        [Fact]
        public void GivenTreeWithItems_Count_ReturnsNumberOfItemsInTree()
        {
            _root.AddChildrenByValues(new Species("B"), new Species("A"));

            Assert.Equal(3, _root.Count());
        }

        [Fact]
        public void GivenTree_Count_ReturnsNumberOfFilteredItemsInTree()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.Equal(1, _root.Count(t => t.Value.Name == "B"));
        }

        [Fact]
        public void GivenTree_CountByValue_ReturnsNumberOfFilteredItemsInTree()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.Equal(1, _root.CountByValue(t => t.Name == "B"));
        }

        [Fact]
        public void GivenTreeWithWantedItems_Any_ReturnsTrue()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.True(_root.Any(t => t.Value.Equals(new Species("A"))));
        }

        [Fact]
        public void GivenTreeWithoutWantedItems_Any_ReturnsFalse()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.False(_root.Any(t => t.Value.Equals(new Species("C"))));
        }

        [Fact]
        public void GivenTreeWithWantedItems_AnyByValue_ReturnsTrue()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.True(_root.AnyByValue(s => s.Equals(new Species("A"))));
        }

        [Fact]
        public void GivenTreeWithoutWantedItems_AnyByValue_ReturnsFalse()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.False(_root.AnyByValue(s => s.Equals(new Species("C"))));
        }

        [Fact]
        public void GivenTreeWithWantedItems_All_ReturnsTrue()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.True(_root.All(t => t.Value.Counter == 0));
        }

        [Fact]
        public void GivenTreeWithoutWantedItems_All_ReturnsFalse()
        {
            _root.AddChildrenByValues(
                new Species("A"), 
                new Species("B") { Counter = 1 });

            Assert.False(_root.All(t => t.Value.Counter == 0));
        }

        [Fact]
        public void GivenTreeWithWantedItems_AllByValue_ReturnsTrue()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.True(_root.AllByValue(s => s.Counter == 0));
        }

        [Fact]
        public void GivenTreeWithoutWantedItems_AllByValue_ReturnsFalse()
        {
            _root.AddChildrenByValues(
                new Species("A"), 
                new Species("B") { Counter = 1 });

            Assert.False(_root.AllByValue(s => s.Counter == 0));
        }

        [Fact]
        public void GivenTreeWithItems_Sum_ReturnsSum()
        {
            _root.AddChildrenByValues(
                new Species("A") { Counter = 3 }, 
                new Species("B") { Counter = 5 });

            Assert.Equal(8, _root.Sum(t => t.Value.Counter));
        }

        [Fact]
        public void GivenTreeWithItems_SumByValue_ReturnsSum()
        {
            _root.AddChildrenByValues(
                new Species("A") { Counter = 3 }, 
                new Species("B") { Counter = 5 });

            Assert.Equal(8, _root.SumByValue(s => s.Counter));
        }

        [Fact]
        public void GivenTreeWithItems_Max_ReturnsSum()
        {
            _root.AddChildrenByValues(
                new Species("A") { Counter = 3 }, 
                new Species("B") { Counter = 5 });

            Assert.Equal(5, _root.Max(t => t.Value.Counter));
        }

        [Fact]
        public void GivenTreeWithItems_MaxByValue_ReturnsSum()
        {
            _root.AddChildrenByValues(
                new Species("A") { Counter = 3 }, 
                new Species("B") { Counter = 5 });

            Assert.Equal(5, _root.MaxByValue(s => s.Counter));
        }

        [Fact]
        public void GivenTreeWithItems_Min_ReturnsSum()
        {
            _root.AddChildrenByValues(
                new Species("A") { Counter = 3 }, 
                new Species("B") { Counter = 5 });

            Assert.Equal(0, _root.Min(t => t.Value.Counter));
        }

        [Fact]
        public void GivenTreeWithItems_MinByValue_ReturnsSum()
        {
            _root.AddChildrenByValues(
                new Species("A") { Counter = 3 }, 
                new Species("B") { Counter = 5 });

            Assert.Equal(0, _root.MinByValue(s => s.Counter));
        }

        [Fact]
        public void GivenTreeWithItems_Average_ReturnsSum()
        {
            _root.AddChildrenByValues(
                new Species("A") { Counter = 3 }, 
                new Species("B") { Counter = 6 });

            Assert.Equal(3, _root.Average(t => t.Value.Counter));
        }

        [Fact]
        public void GivenTreeWithItems_AverageByValue_ReturnsSum()
        {
            _root.AddChildrenByValues(
                new Species("A") { Counter = 3 }, 
                new Species("B") { Counter = 6 });

            Assert.Equal(3, _root.AverageByValue(s => s.Counter));
        }

        [Fact]
        public void GivenTreeWithItems_First_ReturnsFirstMatchingItem()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.Equal(
                Tree(new Species("A")), 
                _root.First(t => t.Value.Name == "A"));
        }

        [Fact]
        public void GivenTreeWithItems_First_ThrowsExceptionWhenNoItemMatches()
        {
            var exception = Assert.Throws<InvalidOperationException>(
                () => _root.First(t => t.Value.Name == "non-existing"));
            Assert.Equal("Sequence contains no matching element", exception.Message);
        }

        [Fact]
        public void GivenTreeWithItems_FirstByValue_ReturnsFirstMatchingItem()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.Equal(new Species("A"), _root.FirstByValue(s => s.Name == "A"));
        }

        [Fact]
        public void GivenTreeWithItems_FirstByValue_ThrowsExceptionWhenNoItemMatches()
        {
            var exception = Assert.Throws<InvalidOperationException>(
                () => _root.FirstByValue(s => s.Name == "non-existing"));
            Assert.Equal("Sequence contains no matching element", exception.Message);
        }

        [Fact]
        public void GivenTreeWithItems_FirstOrDefault_ReturnsFirstMatchingItem()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.Equal(
                Tree(new Species("A")), 
                _root.FirstOrDefault(t => t.Value.Name == "A"));
        }

        [Fact]
        public void GivenTreeWithItems_FirstOrDefault_ReturnsNullWhenNoItemMatches()
        {
            Assert.Null(_root.FirstOrDefault(t => t.Value.Name == "non-existing"));
        }

        [Fact]
        public void GivenTreeWithItems_FirstOrDefaultByValue_ReturnsFirstMatchingItem()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.Equal(
                new Species("A"), 
                _root.FirstOrDefaultByValue(s => s.Name == "A"));
        }

        [Fact]
        public void GivenTreeWithItems_FirstOrDefaultByValue_ReturnsNullWhenNoItemMatches()
        {
            Assert.Null(_root.FirstOrDefaultByValue(s => s.Name == "non-existing"));
        }

        [Fact]
        public void GivenTreeWithItems_Single_ReturnsSingleMatchingItem()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.Equal(
                Tree(new Species("A")), 
                _root.Single(t => t.Value.Name == "A"));
        }

        [Fact]
        public void GivenTreeWithItems_Single_ThrowsExceptionWhenNoItemMatches()
        {
           var exception =  Assert.Throws<InvalidOperationException>(
                () => _root.Single(t => t.Value.Name == "non-existing"));
            Assert.Equal("Sequence contains no matching element", exception.Message);
        }

        [Fact]
        public void GivenTreeWithItems_Single_ThrowsExceptionWhenMultipleItemsMatch()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            var exception = Assert.Throws<InvalidOperationException>(
                () => _root.Single(t => t.Value.Counter == 0));
            Assert.Equal("Sequence contains more than one matching element", exception.Message);
        }

        [Fact]
        public void GivenTreeWithItems_SingleByValue_ReturnsSingleMatchingItem()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.Equal(new Species("A"), _root.SingleByValue(s => s.Name == "A"));
        }

        [Fact]
        public void GivenTreeWithItems_SingleByValue_ThrowsExceptionWhenNoItemMatches()
        {
            var exception = Assert.Throws<InvalidOperationException>(
                () => _root.SingleByValue(s => s.Name == "non-existing"));
            Assert.Equal("Sequence contains no matching element", exception.Message);
        }

        [Fact]
        public void GivenTreeWithItems_SingleByValue_ThrowsExceptionWhenMultipleItemsMatch()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            var exception = Assert.Throws<InvalidOperationException>(
                () => _root.SingleByValue(s => s.Counter == 0));
            Assert.Equal("Sequence contains more than one matching element", exception.Message);
        }

        [Fact]
        public void GivenTreeWithItems_SingleOrDefault_ReturnsSingleMatchingItem()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.Equal(
                Tree(new Species("A")), 
                _root.SingleOrDefault(t => t.Value.Name == "A"));
        }

        [Fact]
        public void GivenTreeWithItems_SingleOrDefault_ReturnsNull()
        {
            Assert.Null(_root.SingleOrDefault(t => t.Value.Name == "non-existing"));
        }

        [Fact]
        public void GivenTreeWithItems_SingleOrDefault_ThrowsExceptionWhenMultipleItemsMatch()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            var exception = Assert.Throws<InvalidOperationException>(
                () => _root.SingleOrDefault(t => t.Value.Counter == 0));
            Assert.Equal("Sequence contains more than one matching element", exception.Message);
        }
        
        [Fact]
        public void GivenTreeWithItems_SingleOrDefaultByValue_ReturnsSingleMatchingItem()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.Equal(
                new Species("A"), 
                _root.SingleOrDefaultByValue(s => s.Name == "A"));
        }

        [Fact]
        public void GivenTreeWithItems_SingleOrDefaultByValue_ThrowsExceptionWhenNoItemMatches()
        {
            Assert.Null(_root.SingleOrDefaultByValue(s => s.Name == "non-existing"));
        }

        [Fact]
        public void GivenTreeWithItems_SingleOrDefaultByValue_ThrowsExceptionWhenMultipleItemsMatch()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            var exception = Assert.Throws<InvalidOperationException>(
                () => _root.SingleOrDefaultByValue(s => s.Counter == 0));
            Assert.Equal("Sequence contains more than one matching element", exception.Message);
        }

        [Fact]
        public void GivenTwoEqualRootOnlyItems_Equals_ReturnsTrue()
        {
            var tree1 = Tree(new Species("A"));
            var tree2 = Tree(new Species("A"));

            Assert.True(tree1.Equals(tree2));
        }

        [Fact]
        public void GivenTwoUnequalRootOnlyItems_Equals_ReturnsFalse()
        {
            var tree1 = Tree(new Species("A"));
            var tree2 = Tree(new Species("B"));

            Assert.False(tree1.Equals(tree2));
        }

        [Fact]
        public void GivenTwoEqualItemsWithEqualChildren_Equals_ReturnsTrue()
        {
            var tree1 = Tree(new Species("root"));
            tree1.AddChildrenByValues(new Species("A"), new Species("B"));

            var tree2 = Tree(new Species("root"));
            tree2.AddChildrenByValues(new Species("A"), new Species("B"));

            Assert.True(tree1.Equals(tree2));
        }

        [Fact]
        public void GivenTwoEqualItemsWithUnequalChildren_Equals_ReturnsFalse()
        {
            var tree1 = Tree(new Species("root"));
            tree1.AddChildrenByValues(new Species("A"), new Species("B"));

            var tree2 = Tree(new Species("root"));
            tree2.AddChildrenByValues(new Species("A"), new Species("C"));

            Assert.False(tree1.Equals(tree2));
        }

        [Fact]
        public void GivenSerializableTreeWithItems_Clone_ReturnsIdenticalCopy()
        {
            _root.AddChildrenByValues(new Species("A"), new Species("B"));

            var clone = _root.Clone();

            Assert.Equal(_root, clone);
            Assert.NotSame(_root, clone);
        }

        private static Tree<Species> Tree(Species species)
            => Tree<Species>.Grow(species, t => t.Subspecies);

        private static Tree<Species> Tree(Species species, params Species[] subspecies)
            => new Tree<Species>(species, subspecies);
    }
}
