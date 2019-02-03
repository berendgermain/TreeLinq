/*
 * Name         : Tree
 * Project      : TreeLinq
 * Description  : Attaching Linq-like functionality to hierarchically oriented classes.
 * Author       : Berend Germain
 * License      : https://opensource.org/licenses/MIT
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;

namespace TreeLinq
{
    [Serializable]
    public class Tree<TValue> : IEquatable<Tree<TValue>>
    {
        private readonly List<Tree<TValue>> _children = new List<Tree<TValue>>();

        public Tree()
        {
        }

        public Tree(TValue value)
        {
            Value = value;
        }

        public Tree(TValue value, params TValue[] children)
            : this(value, children.AsEnumerable())
        {
        }

        public Tree(TValue value, IEnumerable<TValue> children)
        {
            Value = value;
            AddChildrenByValues(children);
        }

        public Tree(TValue value, params Tree<TValue>[] children)
            : this(value, children.AsEnumerable())
        {
        }

        public Tree(TValue value, IEnumerable<Tree<TValue>> children)
        {
            Value = value;
            AddChildren(children);
        }

        public TValue Value { get; }

        public Tree<TValue> Parent { get; private set; }

        public Tree<TValue> Root
        {
            get
            {
                var tree = this;
                while (!tree.IsRoot)
                {
                    tree = tree.Parent;
                }

                return tree;
            }
        }

        public bool IsRoot 
            => Parent is null;

        public bool IsLeaf
            => !_children.Any();

        public IEnumerable<Tree<TValue>> Children
            => _children;

        public IEnumerable<TValue> ChildrenValues
            => _children.Select(t => t.Value);

        public bool HasChildren
            => _children.Any();

        public bool IsFirstChild
            => IsRoot
                ? throw new InvalidOperationException("Root is not a child")
                : Parent.Children.First().Equals(this);

        public bool IsLastChild
            => IsRoot
                ? throw new InvalidOperationException("Root is not a child")
                : Parent.Children.Last().Equals(this);

        public IEnumerable<Tree<TValue>> Siblings
            => IsRoot
                ? Enumerable.Empty<Tree<TValue>>()
                : Parent.Children.Where(c => !c.Equals(this));

        public IEnumerable<TValue> SiblingValues
            => Siblings.Select(s => s.Value);

        public bool HasSiblings
            => !IsRoot && Parent.Children.Count() > 1;

        public Tree<TValue> NextSibling
        {
            get
            {
                if (IsRoot)
                {
                    throw new InvalidOperationException("Root can not be a sibling");
                }

                if (IsLastChild)
                {
                    throw new InvalidOperationException("Can not move past last item");
                }

                var children = Parent.Children.ToList();
                return children[children.IndexOf(this) + 1];
            }
        }

        public Tree<TValue> PreviousSibling
        {
            get
            {
                if (IsRoot)
                {
                    throw new InvalidOperationException("Root can not be a sibling");
                }

                if (IsFirstChild)
                {
                    throw new InvalidOperationException("Can not move before first item");
                }

                var children = Parent.Children.ToList();
                return children[children.IndexOf(this) - 1];
            }
        }

        public int GenerationDepth
            => IsRoot ? 0 : Parent.GenerationDepth + 1;

        public int GenerationCount
            => Max(t => t.GenerationDepth) + 1;

        public IEnumerable<IReadOnlyList<Tree<TValue>>> Generations
            => Enumerable
                .Range(0, GenerationCount)
                .Select(GetGeneration);

        public IEnumerable<Tree<TValue>> Path
        {
            get
            {
                var result = new List<Tree<TValue>>();
                var node = this;
                while (node != null)
                {
                    result.Add(node);
                    node = node.Parent;
                }

                result.Reverse();
                return result;
            }
        }

        public IEnumerable<TValue> PathValues
            => Path.Select(t => t.Value);

        public void AddChild<TTree>(TTree child)
            where TTree : Tree<TValue>
        {
            child.Parent = this;
            _children.Add(child);
        }

        public Tree<TValue> AddChildByValue(TValue value)
        {
            var result = new Tree<TValue>(value)
            {
                Parent = this
            };

            _children.Add(result);

            return result;
        }

        public void AddChildren<TTree>(params TTree[] children)
            where TTree : Tree<TValue>
        {
            AddChildren(children.AsEnumerable());
        }

        public void AddChildren<TTree>(IEnumerable<TTree> children)
            where TTree : Tree<TValue>
        {
            if (children is null)
            {
                return;
            }

            foreach (var child in children)
            {
                AddChild(child);
            }
        }

        public void AddChildrenByValues(IEnumerable<TValue> children)
        {
            if (children is null)
            {
                return;
            }

            foreach (var child in children)
            {
                AddChildByValue(child);
            }
        }

        public void AddChildrenByValues(params TValue[] children)
        {
            AddChildrenByValues(children.AsEnumerable());
        }

        public void RemoveFromParent()
        {
            if (IsRoot)
            {
                throw new InvalidOperationException("Can not remove root from parent");
            }

            Parent.RemoveChild(this);
        }

        public void RemoveChild<TTree>(TTree child)
            where TTree : Tree<TValue>
        {
            _children.Remove(child);
        }

        public void RemoveChildren<TTree>(params TTree[] children)
            where TTree : Tree<TValue>
        {
            RemoveChildren(children.AsEnumerable());
        }

        public void RemoveChildren<TTree>(IEnumerable<TTree> children)
            where TTree : Tree<TValue>
        {
            _children.RemoveAll(children.Contains);
        }

        public void RemoveChildren<TTree>(Func<TTree, bool> selector)
            where TTree : Tree<TValue>
        {
            _children.RemoveAll(t => selector?.Invoke(t as TTree) ?? true);
        }

        public void RemoveAllChildren()
        {
            _children.RemoveAll(t => true);
        }

        public bool IsParentOf(Tree<TValue> tree)
            => tree.Parent?.Equals(this) ?? false;

        public bool IsChildOf(Tree<TValue> tree)
            => Parent?.Equals(tree) ?? false;

        public bool IsSiblingOf(Tree<TValue> tree)
            => Siblings.Contains(tree);

        public static Tree<TValue> Grow(
            TValue tree, 
            Func<TValue, IEnumerable<TValue>> readChildren,
            Expression<Func<IEnumerable<TValue>, IOrderedEnumerable<TValue>>> orderChildren = null) 
            => Grow(tree, t => t, readChildren, orderChildren);

        public static Tree<TResult> Grow<TSource, TResult>(
            TSource tree, 
            Func<TSource, TResult> readNode, 
            Func<TSource, IEnumerable<TSource>> readChildren,
            Expression<Func<IEnumerable<TSource>, IOrderedEnumerable<TSource>>> orderChildren = null)
        {
            if (readChildren is null)
            {
                throw new ArgumentNullException(nameof(readChildren));
            }

            var result = new Tree<TResult>(readNode(tree));
            var children = readChildren(tree);
            var orderedChildren = orderChildren?.Compile()(children).ToList() ?? children;

            foreach (var child in orderedChildren)
            {
                result.AddChild(Grow(child, readNode, readChildren));
            }

            return result;
        }

        public void TraverseDepthFirst(Action<Tree<TValue>> action, Func<Tree<TValue>, bool> skip = null)
        {
            TraverseDepthFirst(this, action, skip);
        }

        public static void TraverseDepthFirst(
            Tree<TValue> tree, 
            Action<Tree<TValue>> action,
            Func<Tree<TValue>, bool> skip = null)
        {
            if (skip?.Invoke(tree) ?? false)
            {
                return;
            }

            action(tree);

            foreach (var child in tree.Children)
            {
                child.TraverseDepthFirst(action, skip);
            }
        }

        public void TraverseBreadthFirst(Action<Tree<TValue>> action, Func<Tree<TValue>, bool> skip = null)
        {
            TraverseBreadthFirst(this, action, skip);
        }

        public static void TraverseBreadthFirst(
            Tree<TValue> tree, 
            Action<Tree<TValue>> action,
            Func<Tree<TValue>, bool> skip = null)
        {
            foreach (var generation in tree.Generations)
            {
                foreach (var generationMember in generation)
                {
                    if (!skip?.Invoke(generationMember) ?? true)
                    {
                        action(generationMember);
                    }
                }
            }
        }

        public bool Contains(Tree<TValue> tree) 
            => Any(t => t.Equals(tree));

        public bool ContainsByValue(TValue value) 
            => AnyByValue(i => i.Equals(value));

        public IEnumerable<Tree<TValue>> ToListDepthFirst()
        {
            var result = new List<Tree<TValue>>();
            TraverseDepthFirst(t => result.Add(t));
            return result;
        }

        public IEnumerable<TProperty> ToListDepthFirst<TProperty>(Func<Tree<TValue>, TProperty> selector)
            => ToListDepthFirst().Select(selector);

        public IEnumerable<TValue> ToListValuesDepthFirst()
        {
            var result = new List<TValue>();
            TraverseDepthFirst(t => result.Add(t.Value));
            return result;
        }

        public IEnumerable<TProperty> ToListValuesDepthFirst<TProperty>(Func<TValue, TProperty> selector)
            => ToListValuesDepthFirst().Select(selector);

        public IEnumerable<Tree<TValue>> ToListBreadthFirst()
        {
            var result = new List<Tree<TValue>>();
            TraverseBreadthFirst(t => result.Add(t));
            return result;
        }

        public IEnumerable<TProperty> ToListBreadthFirst<TProperty>(Func<Tree<TValue>, TProperty> selector)
            => ToListBreadthFirst().Select(selector);

        public IEnumerable<TValue> ToListValuesBreadthFirst()
        {
            var result = new List<TValue>();
            TraverseBreadthFirst(t => result.Add(t.Value));
            return result;
        }

        public IEnumerable<TProperty> ToListValuesBreadthFirst<TProperty>(Func<TValue, TProperty> selector)
            => ToListValuesBreadthFirst().Select(selector);

        public IReadOnlyList<Tree<TValue>> GetGeneration(int index)
        {
            if (index == 0)
            {
                return new List<Tree<TValue>> { Root };
            }

            var result = new List<Tree<TValue>>();

            TraverseDepthFirst(
                t =>
                {
                    if (t.GenerationDepth == index)
                    {
                        result.Add(t);
                    }
                },
                t => t.GenerationDepth > index);

            return result;
        }

        public Tree<TValue> GetByChildIndexesPath(params int[] indexes)
        {
            var tree = Root;
            foreach (var index in indexes)
            {
                var children = tree.Children.ToList();
                if (index >= children.Count)
                {
                    throw new IndexOutOfRangeException($"Index '{index}' out of range");
                }

                tree = children.ToList()[index];
            }

            return tree;
        }

        public Tree<TValue> GetByGenerationCoordinates(int depth, int generationIndex)
        {
            if (depth >= GenerationCount)
            {
                throw new ArgumentOutOfRangeException(nameof(depth));
            }

            var generation = GetGeneration(depth);

            if (generationIndex >= generation.Count)
            {
                throw new IndexOutOfRangeException(nameof(generationIndex));
            }

            return generation[generationIndex];
        }

        public Tree<TResult> Select<TResult>(
            Func<Tree<TValue>, TResult> selector,
            Expression<Func<IEnumerable<Tree<TValue>>, IOrderedEnumerable<Tree<TValue>>>> orderChildren = null)
        {
            var result = new Tree<TResult>(selector(this));
            var orderedChildren = orderChildren?.Compile()(Children).ToList() ?? Children;

            foreach (var child in orderedChildren)
            {
                result.AddChild(child.Select(selector));
            }

            return result;
        }

        public Tree<TResult> SelectByValue<TResult>(
            Func<TValue, TResult> selector,
            Expression<Func<IEnumerable<Tree<TValue>>, IOrderedEnumerable<Tree<TValue>>>> orderChildren = null)
        {
            var result = new Tree<TResult>(selector(Value));
            var orderedChildren = orderChildren?.Compile()(Children).ToList() ?? Children;

            foreach (var child in orderedChildren)
            {
                result.AddChild(child.SelectByValue(selector));
            }

            return result;
        }

        public List<Tree<TValue>> Where(Func<Tree<TValue>, bool> selector)
        {
            var list = new List<Tree<TValue>>();

            TraverseDepthFirst(
                t => 
                {
                    if (selector(t))
                    {
                        list.Add(t);
                    }
                });

            return list;
        }

        public List<TValue> WhereByValue(Func<TValue, bool> selector)
        {
            var list = new List<TValue>();

            TraverseDepthFirst(
                t => 
                {
                    if (selector(t.Value))
                    {
                        list.Add(t.Value);
                    }
                });

            return list;
        }

        public int Count()
            => 1 + Children.Sum(child => child.Count());

        public int Count(Func<Tree<TValue>, bool> selector)
            => (selector(this) ? 1 : 0) + Children.Sum(c => c.Count(selector));

        public int CountByValue(Func<TValue, bool> selector)
            => (selector(Value) ? 1 : 0) + Children.Sum(c => c.CountByValue(selector));

        public bool Any(Func<Tree<TValue>, bool> selector)
            => selector(this) || Children.Any(c => c.Any(selector));

        public bool AnyByValue(Func<TValue, bool> selector)
            => selector(Value) || Children.Any(c => c.AnyByValue(selector));

        public bool All(Func<Tree<TValue>, bool> selector)
            => selector(this) && Children.All(c => c.All(selector));

        public bool AllByValue(Func<TValue, bool> selector)
            => selector(Value) && Children.All(c => c.AllByValue(selector));

        public int Sum(Func<Tree<TValue>, int> selector) 
            => ToListDepthFirst(selector).Sum();

        public int SumByValue(Func<TValue, int> selector) 
            => ToListValuesDepthFirst(selector).Sum();

        public long Sum(Func<Tree<TValue>, long> selector) 
            => ToListDepthFirst(selector).Sum();

        public long SumByValue(Func<TValue, long> selector) 
            => ToListValuesDepthFirst(selector).Sum();

        public decimal Sum(Func<Tree<TValue>, decimal> selector) 
            => ToListDepthFirst(selector).Sum();

        public decimal SumByValue(Func<TValue, decimal> selector) 
            => ToListValuesDepthFirst(selector).Sum();

        public double Sum(Func<Tree<TValue>, double> selector) 
            => ToListDepthFirst(selector).Sum();

        public double SumByValue(Func<TValue, double> selector) 
            => ToListValuesDepthFirst(selector).Sum();

        public float Sum(Func<Tree<TValue>, float> selector) 
            => ToListDepthFirst(selector).Sum();

        public float SumByValue(Func<TValue, float> selector) 
            => ToListValuesDepthFirst(selector).Sum();

        public int Max(Func<Tree<TValue>, int> selector)
            => ToListDepthFirst(selector).Max();

        public int MaxByValue(Func<TValue, int> selector)
            => ToListValuesDepthFirst(selector).Max();

        public long Max(Func<Tree<TValue>, long> selector)
            => ToListDepthFirst(selector).Max();

        public long MaxByValue(Func<TValue, long> selector)
            => ToListValuesDepthFirst(selector).Max();

        public decimal Max(Func<Tree<TValue>, decimal> selector)
            => ToListDepthFirst(selector).Max();

        public decimal MaxByValue(Func<TValue, decimal> selector)
            => ToListValuesDepthFirst(selector).Max();

        public double Max(Func<Tree<TValue>, double> selector)
            => ToListDepthFirst(selector).Max();

        public double MaxByValue(Func<TValue, double> selector)
            => ToListValuesDepthFirst(selector).Max();

        public float Max(Func<Tree<TValue>, float> selector)
            => ToListDepthFirst(selector).Max();

        public float MaxByValue(Func<TValue, float> selector)
            => ToListValuesDepthFirst(selector).Max();

        public int Min(Func<Tree<TValue>, int> selector)
            => ToListDepthFirst(selector).Min();

        public int MinByValue(Func<TValue, int> selector) 
            => ToListValuesDepthFirst(selector).Min();

        public long Min(Func<Tree<TValue>, long> selector)
            => ToListDepthFirst(selector).Min();

        public long MinByValue(Func<TValue, long> selector)
            => ToListValuesDepthFirst(selector).Min();

        public decimal MinByValue(Func<TValue, decimal> selector)
            => ToListValuesDepthFirst(selector).Min();

        public decimal Min(Func<Tree<TValue>, decimal> selector)
            => ToListDepthFirst(selector).Min();

        public double Min(Func<Tree<TValue>, double> selector)
            => ToListDepthFirst(selector).Min();

        public double MinByValue(Func<TValue, double> selector)
            => ToListValuesDepthFirst(selector).Min();

        public float Min(Func<Tree<TValue>, float> selector)
            => ToListDepthFirst(selector).Min();

        public float MinByValue(Func<TValue, float> selector)
            => ToListValuesDepthFirst(selector).Min();

        public double Average(Func<Tree<TValue>, int> selector) 
            => ToListDepthFirst(selector).Average();

        public double AverageByValue(Func<TValue, int> selector) 
            => ToListValuesDepthFirst(selector).Average();

        public double Average(Func<Tree<TValue>, long> selector) 
            => ToListDepthFirst(selector).Average();

        public double AverageByValue(Func<TValue, long> selector) 
            => ToListValuesDepthFirst(selector).Average();

        public decimal Average(Func<Tree<TValue>, decimal> selector) 
            => ToListDepthFirst(selector).Average();

        public decimal AverageByValue(Func<TValue, decimal> selector) 
            => ToListValuesDepthFirst(selector).Average();

        public double Average(Func<Tree<TValue>, double> selector) 
            => ToListDepthFirst(selector).Average();

        public double AverageByValue(Func<TValue, double> selector) 
            => ToListValuesDepthFirst(selector).Average();

        public double Average(Func<Tree<TValue>, float> selector) 
            => ToListDepthFirst(selector).Average();

        public float AverageByValue(Func<TValue, float> selector) 
            => ToListValuesDepthFirst(selector).Average();

        public Tree<TValue> First(Func<Tree<TValue>, bool> selector) 
            => selector(this) 
                ? this
                : Children.First(selector);

        public TValue FirstByValue(Func<TValue, bool> selector) 
            => selector(Value) 
                ? Value
                : Children.Select(t => t.Value).First(selector);

        public Tree<TValue> FirstOrDefault(Func<Tree<TValue>, bool> selector) 
            => selector(this) 
                ? this
                : Children.FirstOrDefault(selector);

        public TValue FirstOrDefaultByValue(Func<TValue, bool> selector) 
            => selector(Value) 
                ? Value
                : Children.Select(t => t.Value).FirstOrDefault(selector);

        public Tree<TValue> Single(Func<Tree<TValue>, bool> selector) 
            => ToListDepthFirst().Single(selector);

        public TValue SingleByValue(Func<TValue, bool> selector) 
            => ToListValuesDepthFirst().Single(selector);

        public Tree<TValue> SingleOrDefault(Func<Tree<TValue>, bool> selector) 
            => ToListDepthFirst().SingleOrDefault(selector);

        public TValue SingleOrDefaultByValue(Func<TValue, bool> selector) 
            => ToListValuesDepthFirst().SingleOrDefault(selector);

        public override int GetHashCode() 
            => Value.GetHashCode() ^ Children.GetHashCode();

        public override bool Equals(object obj)
            => Equals(obj as Tree<TValue>);

        public bool Equals(Tree<TValue> other)
            => other != null && Value.Equals(other.Value) &&
               GetType() == other.GetType() && 
               Children.SequenceEqual(other.Children);

        public Tree<TValue> Clone()
            => Clone<Tree<TValue>>();

        public TTree Clone<TTree>()
            where TTree : Tree<TValue>
        {
            var valueType = typeof(TValue);
            if (!valueType.IsSerializable)
            {
                throw new ArgumentException($"Tree value type '{valueType.FullName}' is not serializable");
            }

            using (var stream = new MemoryStream()) {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
                stream.Position = 0;
                var clone = formatter.Deserialize(stream);
                stream.Close();
                return (TTree)clone;
            }
        }
    }
}
