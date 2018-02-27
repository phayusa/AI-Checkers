using System;
using System.Collections.Generic;

namespace AICheckers
{
    class Tree<T>
    {
        private readonly List<Tree<T>> _children;
        private readonly T _value;

        public Tree(T value)
        {
            _value = value;
            _children = new List<Tree<T>>();
        }

        public Tree<T> AddChild(T value)
        {
            Tree<T> child = new Tree<T>(value) {Parent = this};
            _children.Add(child);
            return child;
        }

        public void Traverse(Action<T> visitor)
        {
            TraversePrivate(visitor);
        }

        private void TraversePrivate(Action<T> visitor)
        {
            visitor(_value);
            foreach (Tree<T> child in _children)
                child.TraversePrivate(visitor);
        }

        private Tree<T> Parent
        {
            get;
            set;
        }

        public List<Tree<T>> Children => _children;

        public T Value => _value;
    }
}
