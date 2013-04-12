using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SurfaceApplication2
{
    public class Movie : INotifyPropertyChanged
    {
        private string title;
        private string imageSource;
        private double price;

        public event PropertyChangedEventHandler PropertyChanged;
        private Movie movie;

        public Movie(String imageSource, String title, double price)
        {
            this.title = title;
            this.imageSource = imageSource;
            this.price = price;
        }

        public Movie(Movie movie)
        {
            // TODO: Complete member initialization
            this.title = String.Copy(movie.title);
            this.imageSource = String.Copy(movie.imageSource);
            this.price = movie.price;
        }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

        public string ImageSource
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                OnPropertyChanged("ImageSource");
            }

        }

        public double Price
        {
            get { return price; }
            set
            {
                price = value;
                OnPropertyChanged("Overview");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}

