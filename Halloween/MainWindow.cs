using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.CoreAnimation;

namespace Halloween
{
	public partial class MainWindow : MonoMac.AppKit.NSWindow
	{
		#region Constructors
		// Called when created from unmanaged code
		public MainWindow (IntPtr handle) :  base (handle)
		{
			Initialize ();
		}
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		// Shared initialization code
		void Initialize ()
		{
			BackgroundColor = NSColor.Black;
			//ToggleFullScreen (this); <-- Doesn't work on my Mac Mini running Snow Leopard :(
			SetFrame (Screen.Frame, true, true);
			NSMenu.MenuBarVisible = false;
		}
		#endregion

		public void SetBackgroundImage(NSImage image)
		{
			BackgroundImage.Image = image;
			BackgroundImage.SetFrameSize (image.Size);
			BackgroundImage.SetFrameOrigin (new PointF ( (Frame.Width - image.Size.Width) / 2, (Frame.Height - image.Size.Height) / 2));
			BackgroundImage.AutoresizingMask = NSViewResizingMask.MinXMargin | NSViewResizingMask.MaxXMargin | NSViewResizingMask.MinYMargin | NSViewResizingMask.MaxYMargin;
		}

		public void HideBackgroundImage ()
		{
			BackgroundImage.Image = null;
		}
	}
}