using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.UI
{
    public class BaseUIController
    {
        public VisualElement Parent { get; set; }
        public VisualElement Root { get; set; }

        public VisualElement Initialize(VisualElement parent, VisualTreeAsset source)
        {
            Parent = parent;
            Root = source.Instantiate();

            Parent.Add(Root);
            Root.visible = true;
            Root.userData = this;

            return Root;
        }

        public VisualElement Initialize(VisualElement parent, string sourcePath)
        {
            return Initialize(parent, Resources.Load<VisualTreeAsset>(sourcePath));
        }

        public VisualElement Initialize(VisualElement parent, VisualTreeAsset source, IEnumerable<string> classList)
        {
            var root = Initialize(parent, source);

            foreach(var c in classList)
            {
                root.AddToClassList(c);
            }

            return root;
        }

        public VisualElement Initialize(VisualElement parent, string sourcePath, IEnumerable<string> classList)
        {           
            return Initialize(parent, Resources.Load<VisualTreeAsset>(sourcePath), classList);
        }
    }
}
