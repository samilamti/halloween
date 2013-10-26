using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Drawing;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace Halloween
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		BackgroundWorker worker = new BackgroundWorker();
		NSSound soundPlayer;
		const int minImage = 1;
		const int maxImage = 29;
		readonly TimeSpan minImageShowingTime = TimeSpan.FromSeconds(5);
		readonly TimeSpan maxImageShowingTime = TimeSpan.FromSeconds(10);
		readonly TimeSpan minRestBetweenImages = TimeSpan.FromSeconds(30);
		readonly TimeSpan maxRestBetweenImages = new TimeSpan (0, 3, 0);
		DateTime begunShowingImage = DateTime.MinValue;
		DateTime begunResting = DateTime.MinValue;
		bool isShowingImage = false;
		const int MSG_HIDE_IMAGE = 100;

		#region Resources
		string[] images = {
			"Scary1.jpg", "Scary10.jpg", "Scary11.jpg", "Scary12.jpg", "Scary13.jpg", "Scary14.jpg", "Scary15.jpg", "Scary16.jpg", 
			"Scary17.jpg", "Scary18.jpg", "Scary19.jpg", "Scary2.jpg", "Scary20.png", "Scary21.jpg", "Scary22.jpg", "Scary22.jpg", 
			"Scary23.png", "Scary24.jpg", "Scary25.jpg", "Scary26.jpg", "Scary27.jpg", "Scary28.jpg", "Scary29.jpg", "Scary3.jpg", 
			"Scary30.jpg", "Scary4.jpg", "Scary5.jpg", "Scary6.jpg", "Scary7.jpg", "Scary8.jpg", "Scary9.png", 
		};
		string[] sounds = {
			"Scary Sounds - 06 - Bones Breaking.mp3", 
			"Scary Sounds - 10 - Cell Door.mp3",
			"Scary Sounds - 11 - Chains.mp3",
			"Scary Sounds - 13 - Chopping.mp3",
			"Scary Sounds - 15 - Creaks.mp3",
			"Scary Sounds - 16 - Dog Barking.mp3",
			"Scary Sounds - 17 - Dog Snarling.mp3",
			"Scary Sounds - 18 - Door Knocks.mp3",
			"Scary Sounds - 19 - Door Creaks.mp3",
			"Scary Sounds - 24 - Eye Popping.mp3",
			"Scary Sounds - 25 - Falling Down Well.mp3",
			"Scary Sounds - 26 - Falling into Chemicals.mp3",
			"Scary Sounds - 27 - Finger Cutting.mp3",
			"Scary Sounds - 30 - Glass Breaking.mp3",
			"Scary Sounds - 31 - Groaning Man.mp3",
			"Scary Sounds - 33 - Guillotine.mp3",
			"Scary Sounds - 40 - Lab Machines.mp3",
			"Scary Sounds - 43 - Lightning.mp3",
			"Scary Sounds - 46 - Monster Breaks Window.mp3",
			"Scary Sounds - 47 - Monster Roar.mp3",
			"Scary Sounds - 48 - Poltergeist.mp3",
			"Scary Sounds - 54 - Strangled Man.mp3",
			"Scary Sounds - 60 - Transylvania Owl.mp3",
			"Scary Sounds - 65 - Witch's Laugh.mp3",
			"Scary Sounds - 66 - Woman Falling.mp3",
			"Scary Sounds - 69 - Wolf.mp3"
		};
		#endregion

		#region Constructors
		// Called when created from unmanaged code
		public MainWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		// Call to load from the XIB/NIB file
		public MainWindowController () : base ("MainWindow")
		{
			Initialize ();
		}
		// Shared initialization code
		void Initialize ()
		{
			worker.DoWork += HandleDoWork;
			worker.WorkerReportsProgress = true;
			worker.ProgressChanged += HandleProgressChanged;
		}

		Random r = new Random();
		void HandleProgressChanged (object sender, ProgressChangedEventArgs e)
		{
			if (e.ProgressPercentage == MSG_HIDE_IMAGE) {
				Window.HideBackgroundImage ();
				if (soundPlayer.IsPlaying())
					soundPlayer.Stop ();
			}
			else {
				Window.SetBackgroundImage (NSImage.ImageNamed (images[e.ProgressPercentage]));
				if (soundPlayer != null) {
					soundPlayer.Dispose ();
				}
				var path = NSBundle.MainBundle.PathForSoundResource (sounds[r.Next(0, sounds.Length)]);
				soundPlayer = new NSSound (path, true);
				soundPlayer.Play ();
			}
		}
		public override void WindowDidLoad ()
		{
			base.WindowDidLoad ();
			worker.RunWorkerAsync ();
		}
		void HandleDoWork (object sender, DoWorkEventArgs e)
		{
			Random r = new Random ();

			while (!e.Cancel) {

				Thread.Sleep (40);

				if (isShowingImage) {
					if (begunShowingImage + minImageShowingTime > DateTime.UtcNow) {
						// We've just recently begun showing an image
					}
					else if (begunShowingImage + maxImageShowingTime < DateTime.UtcNow) {
						// We've overstayed our image viewing
						HideImage ();
					}
					else if (r.Next (1, 4) == 3) {
						HideImage ();
					}
					continue;
				}

				// We're not showing an image
				if (begunResting + minRestBetweenImages > DateTime.UtcNow) {
					// We've just recently begun resting
				} 
				else if (begunResting + maxRestBetweenImages < DateTime.UtcNow) {
					// We've rested for far too long!
					ShowImage (r.Next(minImage, maxImage));
				}
				else if (r.Next(1,4) == 3) {
					ShowImage (r.Next(minImage, maxImage));
				}
				continue;
			}
		}
		void HideImage() {
			worker.ReportProgress (MSG_HIDE_IMAGE);
			isShowingImage = false;
			begunResting = DateTime.UtcNow;
		}

		void ShowImage(int imageIndex) {
			worker.ReportProgress (imageIndex);
			isShowingImage = true;
			begunShowingImage = DateTime.UtcNow;
		}
		#endregion
		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}
	}
}

