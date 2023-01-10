using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AxisRotationExperimenter
{
    public class SliderAndTextEventArgs : EventArgs
    {
        public string SliderName;
        public double SliderValue;

        public SliderAndTextEventArgs (string name, double val)
        {            
            SliderName = name;
            SliderValue = val;
        }
    }

    //******************************************************************************************

    public partial class SliderTextAndLabel : UserControl
    {
        //***************************************************

        // delegate typedef - defines the format of client callbacks for SliderTextAndLabel events
        public delegate void SliderAndTextCallback (SliderTextAndLabel src, SliderAndTextEventArgs args);

        // event that other classes can subscribe to
        public event SliderAndTextCallback ValueChanged;

        //***************************************************

        public SliderTextAndLabel ()
        {
            InitializeComponent ();

          // set properties to their default values. this is only needed if there are properties
          // that don't use data binding
            PropertyChanged (new DependencyPropertyChangedEventArgs ());
        }

        //*********************************************************************************************

        // CLR Property

        public double Value {get {return slider.Value;} set {slider.Value = value;}}

        // Dependency Properties - probably didn't need these to be Dependency Properties, could just be regular CLR properties

        public static readonly DependencyProperty SliderTitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(SliderTextAndLabel),                
                                                                                                     new PropertyMetadata("-----", PropertyChanged));
        public string Title
        {
            set { SetValue(SliderTitleProperty, value); }
            get { return (string)GetValue(SliderTitleProperty); }
        }

        public static readonly DependencyProperty SliderMinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(SliderTextAndLabel),                
                                                                                                     new PropertyMetadata((double)-180, PropertyChanged));
        public double Minimum
        {
            set { SetValue(SliderMinimumProperty, value); }
            get { return (double)GetValue(SliderMinimumProperty); }
        }

        public static readonly DependencyProperty SliderMaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(SliderTextAndLabel),                
                                                                                                     new PropertyMetadata((double) 180, PropertyChanged));
        public double Maximum
        {
            set { SetValue(SliderMaximumProperty, value); }
            get { return (double)GetValue(SliderMaximumProperty); }
        }

        private static void PropertyChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SliderTextAndLabel).PropertyChanged (e);
        }

        private void PropertyChanged (DependencyPropertyChangedEventArgs args)
        {            
            slider.Minimum = Minimum;
            slider.Maximum = Maximum;
        }

        //*********************************************************************************************

        private void slider_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderAndTextEventArgs args = new SliderAndTextEventArgs ((sender as Slider).Name, e.NewValue);
            ValueChanged?.Invoke (this, args);                
        }

        private void textBox_PreviewTextInput (object sender, TextCompositionEventArgs e)
        {
            if (e.Text [e.Text.Length - 1] == '\r')
            {
                e.Handled = true;
                slider.Focus ();
                textBox.Focus ();
            }
        }
    }
}
