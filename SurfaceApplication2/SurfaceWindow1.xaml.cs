using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.Collections.ObjectModel;

namespace SurfaceApplication2
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// 
   
        private ObservableCollection<Movie> scatterItems = new ObservableCollection<Movie>();
        private Movie[] movies = new Movie[3];
        private ObservableCollection<Movie> libraryItems = new ObservableCollection<Movie>();
        private double price = 0;
        private String Price
        {
            get { return "$" + price; }

        }

        public ObservableCollection<Movie> LibraryItems
        {
            get { return libraryItems; }
        }

        public ObservableCollection<Movie> ScatterItems
        {
            get { return scatterItems; }
        }

        public SurfaceWindow1()
        {
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            DataContext = this;

            movies[0] = new Movie("Resources/Blood_waters.jpg", "Zaat", 2.99);
            movies[1] = new Movie("Resources/Manosposter.jpg", "Manos", 4.59);
            movies[2] = new Movie("Resources/Monsteragogo.jpg", "Monster a Go-Go!", 3.96);
            scatterItems.Add(new Movie(movies[0]));
            scatterItems.Add(new Movie(movies[1]));
            scatterItems.Add(new Movie(movies[2]));
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

        private void LibraryStack_Drop(object sender, SurfaceDragDropEventArgs e)
        {
            if(!LibraryItems.Contains(e.Cursor.Data))
            {
                LibraryItems.Add((Movie)e.Cursor.Data);
            }

        }

        private void scatter_Drop(object sender, SurfaceDragDropEventArgs e)
        {
            // If it isn't already on the ScatterView, add it to the source collection.
            if (!ScatterItems.Contains(e.Cursor.Data))
            {
                ScatterItems.Add((Movie)e.Cursor.Data);
            }

            // Get the ScatterViewItem that Scatter automatically generated.
            ScatterViewItem svi =
                scatter.ItemContainerGenerator.ContainerFromItem(e.Cursor.Data) as ScatterViewItem;
            svi.Visibility = System.Windows.Visibility.Visible;
            svi.Width = e.Cursor.Visual.ActualWidth;
            svi.Height = e.Cursor.Visual.ActualHeight;
            svi.Center = e.Cursor.GetPosition(scatter);
            svi.Orientation = e.Cursor.GetOrientation(scatter);
            svi.Background = Brushes.Transparent;
            // Setting e.Handle to true ensures that default behavior is not performed.
            e.Handled = true;
        }

        private void scatter_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            FrameworkElement findSource = e.OriginalSource as FrameworkElement;
            ScatterViewItem draggedElement = null;

            // Find the ScatterViewitem object that is being touched.
            while (draggedElement == null && findSource != null)
            {
                if ((draggedElement = findSource as ScatterViewItem) == null)
                {
                    findSource = VisualTreeHelper.GetParent(findSource) as FrameworkElement;
                }
            }

            if (draggedElement == null)
            {
                return;
            }

            Movie data = draggedElement.Content as Movie;

            // Create the cursor visual
            ContentControl cursorVisual = new ContentControl()
            {
                Content = draggedElement.DataContext,
                Style = FindResource("CursorStyle") as Style
            };

            // Create a list of input devices. Add the touches that
            // are currently captured within the dragged element and
            // the current touch (if it isn't already in the list).
            List<InputDevice> devices = new List<InputDevice>();
            devices.Add(e.TouchDevice);
            foreach (TouchDevice touch in draggedElement.TouchesCapturedWithin)
            {
                if (touch != e.TouchDevice)
                {
                    devices.Add(touch);
                }
            }

            // Get the drag source object
            ItemsControl dragSource = ItemsControl.ItemsControlFromItemContainer(draggedElement);

            SurfaceDragDrop.BeginDragDrop(
                dragSource,                     // The ScatterView object that the cursor is dragged out from.
                draggedElement,                 // The ScatterViewItem object that is dragged from the drag source.
                cursorVisual,                   // The visual element of the cursor.
                draggedElement.DataContext,     // The data attached with the cursor.
                devices,                        // The input devices that start dragging the cursor.
                DragDropEffects.Move);          // The allowed drag-and-drop effects of this operation.

            // Prevents the default touch behavior from happening and disrupting our code.
            e.Handled = true;

            // Hide the ScatterViewItem for now. We will remove it if the DragDrop is successful.
            draggedElement.Visibility = Visibility.Hidden;
        }

        private void scatter_DragCanceled(object sender, SurfaceDragDropEventArgs e)
        {
            Movie data = e.Cursor.Data as Movie;
            ScatterViewItem svi = scatter.ItemContainerGenerator.ContainerFromItem(data) as ScatterViewItem;
            if (svi != null)
            {
                svi.Visibility = Visibility.Visible;
            }
        }

        private void scatter_DragCompleted(object sender, SurfaceDragCompletedEventArgs e)
        {
            Movie data = e.Cursor.Data as Movie;
            price += data.Price;
            totalPriceLabel.Content = Price;
        }
    }
}