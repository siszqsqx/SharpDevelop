﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Provides <see cref="IPlacementBehavior"/> behavior for <see cref="Canvas"/>.
	/// </summary>
	[ExtensionFor(typeof(Canvas))]
	public sealed class CanvasPlacementSupport : BehaviorExtension, IPlacementBehavior
	{
		/// <inherits/>
		protected override void OnInitialized()
		{
			base.OnInitialized();
			this.ExtendedItem.AddBehavior(typeof(IPlacementBehavior), this);
		}
		
		GrayOutDesignerExceptActiveArea grayOut;
		
		/// <inherits/>
		public bool CanPlace(DesignItem child, PlacementType type, PlacementAlignment position)
		{
			return type == PlacementType.Resize || type == PlacementType.Move;
		}
		
		/// <inherits/>
		public void StartPlacement(PlacementOperation operation, out bool supportsRemoveFromContainer)
		{
			supportsRemoveFromContainer = false;
			
			//DesignItemProperty margin = operation.PlacedItem.Properties[FrameworkElement.MarginProperty];
			UIElement child = (UIElement)operation.PlacedItem.Component;
			operation.Left = GetLeft(child);
			operation.Top = GetTop(child);
			operation.Right = operation.Left + GetWidth(child);
			operation.Bottom = operation.Top + GetHeight(child);
			
			GrayOutDesignerExceptActiveArea.Start(ref grayOut, this.Services.GetService<IDesignPanel>(), this.ExtendedItem.View);
		}
		
		static double GetLeft(UIElement element)
		{
			double v = (double)element.GetValue(Canvas.LeftProperty);
			if (double.IsNaN(v))
				return 0;
			else
				return v;
		}
		
		static double GetTop(UIElement element)
		{
			double v = (double)element.GetValue(Canvas.TopProperty);
			if (double.IsNaN(v))
				return 0;
			else
				return v;
		}
		
		static double GetWidth(UIElement element)
		{
			double v = (double)element.GetValue(FrameworkElement.WidthProperty);
			if (double.IsNaN(v))
				return element.RenderSize.Width;
			else
				return v;
		}
		
		static double GetHeight(UIElement element)
		{
			double v = (double)element.GetValue(FrameworkElement.HeightProperty);
			if (double.IsNaN(v))
				return element.RenderSize.Height;
			else
				return v;
		}
		
		/// <inherits/>
		public void UpdatePlacement(PlacementOperation operation)
		{
			DesignItem item = operation.PlacedItem;
			UIElement child = (UIElement)item.Component;
			if (operation.Left != GetLeft(child)) {
				item.Properties.GetAttachedProperty(Canvas.LeftProperty).SetValue(operation.Left);
			}
			if (operation.Top != GetTop(child)) {
				item.Properties.GetAttachedProperty(Canvas.TopProperty).SetValue(operation.Top);
			}
			if (operation.Right - operation.Left != GetWidth(child)) {
				item.Properties.GetProperty(FrameworkElement.WidthProperty).SetValue(operation.Right - operation.Left);
			}
			if (operation.Bottom - operation.Top != GetHeight(child)) {
				item.Properties.GetProperty(FrameworkElement.HeightProperty).SetValue(operation.Bottom - operation.Top);
			}
		}
		
		/// <inherits/>
		public void FinishPlacement(PlacementOperation operation)
		{
			GrayOutDesignerExceptActiveArea.Stop(ref grayOut);
		}
	}
}
