using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Gem.Gui
{
	public class UIItemProperties
	{
		public Dictionary<String, Object> Values;
		public Func<UIItem, bool> Condition;

		public UIItemProperties(Func<UIItem, bool> Condition, Dictionary<String, Object> Values)
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
        public int id;
        public bool Visible = true;
        //public Common.PropertySet defaults;
        //public Common.PropertySet settings;
		public List<UIItemProperties> Properties = new List<UIItemProperties>();
        //public Common.PropertySet hoverSettings;
        public bool Hover { get; set; }
		public UIItem FocusItem { get; set; }
        
        public UIItem root { 
            get { 
				if (parent == null) return this;
                return parent.root; 
			}
		}

        public UIItem(Rectangle rect, Dictionary<String, Object> settings = null)
        {
			if (settings != null) Properties.Add(new UIItemProperties(null, settings));
			else Properties.Add(new UIItemProperties(null, new Dictionary<String, Object>()));

            this.rect = rect;
            Hover = false;
        }

		public void AddPropertySet(Func<UIItem, bool> Condition, Dictionary<String, Object> Values)
		{
			Properties.Insert(0, new UIItemProperties(Condition, Values));
		}

		public static bool IsHover(UIItem item) { return item.Hover; }

        public virtual void HandleMouseEx(bool mouseValid, int x, int y, bool mousePressed, Action<List<Object>> onEvent)
        {
            Hover = mouseValid && rect.Contains(x, y);
            if (Hover && mousePressed)
            {
                var handler = GetSetting("ON-CLICK", null);
                var argumentList = new List<Object>();
                argumentList.Add(handler);
                argumentList.Add(this);
                if (handler != null)
                    onEvent(argumentList);
            }
            if (Visible)
                foreach (var child in children) child.HandleMouseEx(mouseValid, x, y, mousePressed, onEvent);

        }

		public virtual void AddChild(UIItem child)
		{
			children.Add(child);
			child.Properties.Add(this.Properties.Last());
			child.parent = this;
		}
		
		public virtual void RemoveChild(UIItem child)	
        {
            children.Remove(child);
        }

		public void Visit(Action<UIItem> Callback)
		{
			Callback(this);
			foreach (var child in children)
				child.Visit(Callback);
		}
		
		public void Destroy()
		{
			if (parent != null)
				parent.RemoveChild(this);
			parent = null;
		}

		public Dictionary<String, Object> Settings
		{
			get
			{
				if (Properties.Count < 2) return Properties[0].Values;
				else return Properties[Properties.Count - 2].Values;
			}
		}

		public Object GetSetting(String name, Object _default)
		{
			foreach (var propertySet in Properties)
			{
				if (propertySet.Values.ContainsKey(name))
				{
					bool conditionPassed = true;
					if (propertySet.Condition != null) conditionPassed = propertySet.Condition(this);
					if (conditionPassed) return propertySet.Values[name];
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
		
		public virtual void Render(Render.ImmediateMode2d context) 
		{
            if (Visible)
            {
                if (GetSetting("HIDDEN-CONTAINER", null) == null)
                {
					bool ContinueDrawing = true;
					var customDraw = GetSetting("DRAW", null);
					if (customDraw is Func<UIItem, Render.ImmediateMode2d, bool>)
						ContinueDrawing = (customDraw as Func<UIItem, Render.ImmediateMode2d, bool>)(this, context);

					if (ContinueDrawing)
					{
						if (GetSetting("TRANSPARENT", null) == null)
						{
							context.Texture = context.White;
							context.Color = (GetSetting("BG-COLOR", Vector3.One) as Vector3?).Value;
							context.Quad(rect);

							var bgImage = GetSetting("BG-IMAGE", null);
							if (bgImage != null && bgImage is Microsoft.Xna.Framework.Graphics.Texture2D)
							{
								context.Color = (GetSetting("FG-COLOR", Vector3.One) as Vector3?).Value;
								context.Texture = bgImage as Microsoft.Xna.Framework.Graphics.Texture2D;
								context.Glyph(rect.X, rect.Y, rect.Width, rect.Height, 0, 0, 1, 1);
							}
						}

						var label = GetSetting("LABEL", null);
						var font = GetSetting("FONT", null);
						if (label != null && font != null)
						{
							context.Color = (GetSetting("TEXT-COLOR", Vector3.Zero) as Vector3?).Value;
							BitmapFont.RenderText(label.ToString(), rect.X, rect.Y, rect.Width + rect.X,
								(GetSetting("FONT-SCALE", 1.0f) as float?).Value,
								context, font as BitmapFont);
						}
					}
                }

                foreach (var child in children)
                    child.Render(context);
            }
        }

	}

}