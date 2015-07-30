using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Gem.Gui
{
	public class UIItemProperties
	{
		public PropertyBag Values;
		public Func<UIItem, bool> Condition;

		public UIItemProperties(Func<UIItem, bool> Condition, PropertyBag Values)
		{
			this.Values = Values;
			this.Condition = Condition;
		}
	}

    public class UIItem
    {
        public Rectangle rect;
        public List<UIItem> children = new List<UIItem>();
        public UIItem parent;
        public bool Visible = true;
		public List<UIItemProperties> Properties = new List<UIItemProperties>();
        public bool Hover { get; set; }
		public UIItem FocusItem { get; set; }
        
        public UIItem root { 
            get { 
				if (parent == null) return this;
                return parent.root; 
			}
		}

        public UIItem(Rectangle rect, PropertyBag settings)
        {
			if (settings != null) Properties.Add(new UIItemProperties(null, settings));
            this.rect = rect;
            Hover = false;
        }

		public void AddPropertySet(Func<UIItem, bool> Condition, PropertyBag Values)
		{
			Properties.Insert(0, new UIItemProperties(Condition, Values));
		}

		public static bool IsHover(UIItem item) { return item.Hover; }

        public UIItem FindHoverItem(int x, int y)
        {
            foreach (var child in children)
            {
                var item = child.FindHoverItem(x, y);
                if (item != null) return item;
            }

            if ((GetSetting("transparent", false) as bool?).Value) return null;
            if (rect.Contains(x, y)) return this;
            return null;
        }

        public virtual void ClearHover()
        {
            Hover = false;
            foreach (var child in children) child.ClearHover();
        }

		public virtual void AddChild(UIItem child)
		{
			children.Add(child);
			child.parent = this;
		}
		
		public virtual void RemoveChild(UIItem child)	
        {
            children.Remove(child);
        }
        		
		public PropertyBag Settings
		{
			get
			{
				return Properties[0].Values;
			}
		}

		public Object GetSetting(String name, Object _default)
		{
           foreach (var propertySet in Properties)
			{
				if (propertySet.Values.HasProperty(name))
				{
					bool conditionPassed = true;
					if (propertySet.Condition != null) conditionPassed = propertySet.Condition(this);
					if (conditionPassed) return propertySet.Values.GetProperty(name);
				}
			}
			return _default;
		}

        public int GetIntegerSetting(String name, int _default)
        {
            var setting = GetSetting(name, null);
            if (setting == null) return _default;
            try
            {
                return Convert.ToInt32(setting);
            }
            catch (Exception) { return _default; }
        }
		
		public virtual void Render(Gem.Render.RenderContext Context) 
		{
            if (Visible)
            {
                if (!((GetSetting("transparent", false) as bool?).Value))
                {
                    Context.Texture = Context.White;
                    Context.Color = (GetSetting("bg-color", Vector3.One) as Vector3?).Value;
                    Context.ImmediateMode.Quad(rect);
                }

                var label = GetSetting("label", null) as String;
                if (label != null)
                {
                    var font = GetSetting("font", null) as BitmapFont;
                    if (font != null)
                    {
                        Context.Color = (GetSetting("text-color", Vector3.Zero) as Vector3?).Value;
                        BitmapFont.RenderText(label, rect.X, rect.Y, float.PositiveInfinity, 2.0f, Context, font);
                    }
                }

                foreach (var child in children)
                    child.Render(Context);
            }
        }

	}

}