﻿using System;
using System.ComponentModel;
using Bit.iOS.Core.Renderers;
using Bit.iOS.Core.Utilities;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Label), typeof(CustomLabelRenderer))]
namespace Bit.iOS.Core.Renderers
{
    public class CustomLabelRenderer : LabelRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);
			if (Control != null && e.NewElement is Label)
			{
				UpdateFont();
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
            try
            {
				base.OnElementPropertyChanged(sender, e);
			}
			catch (NullReferenceException)
			{
				// Do nothing...
				// There is an issue on Xamarin Forms which throws a null reference
				// https://appcenter.ms/users/kspearrin/apps/bitwarden/crashes/errors/534094859u/overview
				// TODO: Maybe something like this https://github.com/matteobortolazzo/HtmlLabelPlugin/pull/113 can be implemented to avoid this
				// on html labels.
			}

			if (e.PropertyName == Label.FontProperty.PropertyName ||
				e.PropertyName == Label.TextProperty.PropertyName ||
				e.PropertyName == Label.FormattedTextProperty.PropertyName)
			{
				UpdateFont();
			}
		}

		private void UpdateFont()
		{
			if (Element is null || Control is null)
				return;

			var pointSize = iOSHelpers.GetAccessibleFont<Label>(Element.FontSize);
			if (pointSize != null)
			{
				Control.Font = UIFont.FromDescriptor(Element.Font.ToUIFont().FontDescriptor, pointSize.Value);
			}
		}
	}
}
