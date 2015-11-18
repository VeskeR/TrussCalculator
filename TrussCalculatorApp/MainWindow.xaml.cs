using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using Path = System.Windows.Shapes.Path;
using SPoint = System.Windows.Point;

namespace TrussCalculatorApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TrussNode[] _truss;

        private List<string> _nodes = new List<string>();
        private List<string> _joints = new List<string>();

        private double _trussLength;
        private double _trussHeight;
        private double _topNodesPressure;
        private int _anchorNode1;
        private int _anchorNode2;

        private const double _canvasWidth = 400;
        private const double _canvasLeftShift = 100;
        private const double _canvasTopShift = 300;

        public double AnchorPressure
        {
            get
            {
                return
                    _truss.Sum(
                        n =>
                            n.Forces.Where(f => f.Direction.Normalized.Equals(new Vector2(0, -1))).Sum(f => f.Magnitude)) /
                    2;
            }
        }


        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            InitializeInitialTruss();
        }

        private void InitializeInitialTruss()
        {
            LoadTrussSettings(Properties.Resources.BasicTruss.Split('\n').Select(s => s.Trim('\t', '\r')).ToArray());
        }

        private Ellipse GetEllipse(SPoint p)
        {
            Ellipse el = new Ellipse();
            el.Fill = Brushes.Red;
            el.Width = 10;
            el.Height = 10;
            el.Stroke = Brushes.Black;
            el.StrokeThickness = 1;
            el.Margin = new Thickness(-el.Width / 2, -el.Height / 2, 0, 0);
            Canvas.SetLeft(el, p.X);
            Canvas.SetTop(el, p.Y);
            return el;
        }

        private Path GetLine(SPoint start, SPoint end, double thickness = 1, Brush brush = null)
        {
            Path p = new Path
            {
                Data = new PathGeometry(new[] {new PathFigure(start, new[] {new LineSegment(end, true),}, false),}),
                Stroke = brush ?? Brushes.Black,
                StrokeThickness = thickness
            };

            return p;
        }

        private TextBlock GetTextBlock(string content, SPoint position)
        {
            TextBlock text = new TextBlock
            {
                Text = content,
                Foreground = Brushes.Red,
                FontWeight = FontWeights.UltraBold,
                FontSize = 14,
                Height = 16,
                Width = content.Length * 14
            };

            text.Margin = new Thickness(-text.Width / 2, -text.Height / 2, 0, 0);

            Canvas.SetLeft(text, position.X);
            Canvas.SetTop(text, position.Y);

            return text;
        }

        private SPoint TrussPointToCanvasPoint(SPoint p)
        {
            double x = p.X / _trussLength * _canvasWidth + _canvasLeftShift;
            double k = _trussHeight/_trussLength*2;
            double maxHeight = k*_canvasWidth;
            double y = _canvasTopShift - p.Y/_trussHeight*maxHeight/2;

            return new SPoint(x, y);
        }

        private void OpenCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text files|*.txt";

            if (dlg.ShowDialog() == true)
            {
                LoadTrussSettings(File.ReadAllLines(dlg.FileName));
            }
        }

        private void SaveCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Text files|*.txt";

            if (dlg.ShowDialog() == true)
            {
                File.WriteAllLines(dlg.FileName, CreateTrussSettings());
            }
        }

        private void AddNode(object sender, RoutedEventArgs e)
        {
            List<string> ns = _nodes.Select(s => s.Split(' ').ToArray()[0]).ToList();

            string n = (_nodes.Count + 1).ToString();

            double x = double.Parse(TxtBoxNodeX.Text);
            double y = (bool) RdBtnBottomNode.IsChecked
                ? 0
                : x > _trussLength/2 ? 2*_trussHeight - _trussHeight/_trussLength*x*2 : _trussHeight/_trussLength*x*2;

            if (ns.Contains(n))
                MessageBox.Show(string.Format("Ферма уже имеет узел с номером {0}", n));
            else
                _nodes.Add(string.Format("{0} {1} {2}", n, x, y));

            UpdateHelpBox();
        }

        private void AddJoint(object sender, RoutedEventArgs e)
        {
            List<string> ns = _nodes.Select(s => s.Split(' ').ToArray()[0]).ToList();

            string n1 = TxtBoxAddJointN1.Text;
            string n2 = TxtBoxAddJointN2.Text;

            if (!ns.Contains(n1))
                MessageBox.Show(string.Format("Ферма не имеет узла с номером {0}", n1));
            else if (!ns.Contains(n2))
                MessageBox.Show(string.Format("Ферма не имеет узла с номером {0}", n1));
            else
                _joints.Add(string.Format("{0} {1}", TxtBoxAddJointN1.Text, TxtBoxAddJointN2.Text));

            UpdateHelpBox();
        }

        private void LoadTrussSettings(string[] s)
        {
            var lh = s[0].Split(' ').Select(double.Parse).ToArray();

            _trussLength = lh[0];
            _trussHeight = lh[1];

            TxtBoxLength.Text = _trussLength.ToString();
            TxtBoxHeight.Text = _trussHeight.ToString();

            _topNodesPressure = double.Parse(s[1]);

            TxtBoxTopNodesPressure.Text = _topNodesPressure.ToString();

            int n = int.Parse(s[2]);

            _nodes = new List<string>();

            for (int i = 0; i < n; i++)
                _nodes.Add(s[3 + i]);

            int m = int.Parse(s[3 + n]);

            _joints = new List<string>();

            for (int i = 0; i < m; i++)
                _joints.Add(s[4 + n + i]);

            var a1a2 = s[4 + n + m].Split(' ').Select(int.Parse).ToArray();

            _anchorNode1 = a1a2[0];
            _anchorNode2 = a1a2[1];

            TxtBoxAnchorNode1.Text = _anchorNode1.ToString();
            TxtBoxAnchorNode2.Text = _anchorNode2.ToString();

            UpdateHelpBox();
        }

        private string[] CreateTrussSettings()
        {
            List<string> s = new List<string>();

            s.Add(string.Format("{0} {1}", _trussLength, _trussHeight));
            s.Add(_topNodesPressure.ToString());

            s.Add(_nodes.Count.ToString());
            s.AddRange(_nodes);

            s.Add(_joints.Count.ToString());
            s.AddRange(_joints);

            s.Add(string.Format("{0} {1}", _anchorNode1, _anchorNode2));

            return s.ToArray();
        }

        private void ParseTrussSettings()
        {
            _truss = new TrussNode[_nodes.Count];

            foreach (string node in _nodes)
            {
                var ixy = node.Split(' ').ToArray();

                int i = int.Parse(ixy[0]) - 1;
                double x = double.Parse(ixy[1]);
                double y = double.Parse(ixy[2]);

                _truss[i] = new TrussNode(x, y);

                if (_truss[i].Point.Y > 0)
                    _truss[i].Forces.Add(new Force(new Vector2(0, -1), _topNodesPressure));
                else if (_truss[i].Point.X == 0 || _truss[i].Point.X == _trussLength)
                    _truss[i].Forces.Add(new Force(new Vector2(0, -1), _topNodesPressure/2));
            }

            foreach (string joint in _joints)
            {
                var ij = joint.Split(' ').Select(int.Parse).ToArray();

                int i = ij[0] - 1;
                int j = ij[1] - 1;

                TrussJoint jt = new TrussJoint(_truss[i].Point, _truss[j].Point);

                _truss[i].Joints.Add(jt);
                _truss[j].Joints.Add(jt);
            }

            double anchorPressure = AnchorPressure;

            if (_truss.Length > 0)
            {
                _truss[_anchorNode1 - 1].Forces.Add(new Force(new Vector2(0, 1), anchorPressure));
                _truss[_anchorNode2 - 1].Forces.Add(new Force(new Vector2(0, 1), anchorPressure));
            }
        }

        private void UpdateTrussSettings(object sender, RoutedEventArgs e)
        {
            if (!(_trussLength.ToString() == TxtBoxLength.Text && _trussHeight.ToString() == TxtBoxHeight.Text))
            {
                RecalculateTruss(_trussLength, double.Parse(TxtBoxLength.Text), _trussHeight, double.Parse(TxtBoxHeight.Text));
                _trussLength = double.Parse(TxtBoxLength.Text);
                _trussHeight = double.Parse(TxtBoxHeight.Text);
            }

            _topNodesPressure = double.Parse(TxtBoxTopNodesPressure.Text);
            _anchorNode1 = int.Parse(TxtBoxAnchorNode1.Text);
            _anchorNode2 = int.Parse(TxtBoxAnchorNode2.Text);

            UpdateHelpBox();
        }

        private void RecalculateTruss(double oldL, double newL, double oldH, double newH)
        {
            List<string> newNodes = (from n in _nodes
                select n.Split(' ').ToArray()
                into ixy
                let oldX = double.Parse(ixy[1])
                let oldY = double.Parse(ixy[2])
                let newX = oldX/oldL*newL
                let newY = oldY/oldH*newH
                select string.Format("{0} {1} {2}", ixy[0], newX, newY)).ToList();

            _nodes = newNodes;
        }

        private void UpdateHelpBox()
        {
            TxtBoxTruss.Text = "";

            TxtBoxTruss.AppendText(string.Format("Длина фермы: {0}\r\n", _trussLength));
            TxtBoxTruss.AppendText(string.Format("Высота фермы: {0}\r\n", _trussHeight));
            TxtBoxTruss.AppendText(string.Format("Давление в верхних узлах: {0}\r\n", _topNodesPressure));

            TxtBoxTruss.AppendText(string.Format("Количество узлов: {0}\r\n", _nodes.Count));
            foreach (var ixy in _nodes.Select(n => n.Split(' ').ToArray()))
                TxtBoxTruss.AppendText(string.Format("Узел № {0}: x={1} y={2}\r\n", ixy[0], ixy[1], ixy[2]));

            TxtBoxTruss.AppendText(string.Format("Количество соединений: {0}\r\n", _joints.Count));
            for (int i = 0; i < _joints.Count; i++)
                TxtBoxTruss.AppendText(string.Format("Соединение № {0}: {1}\r\n", i + 1, _joints[i]));

            TxtBoxTruss.AppendText(string.Format("Номера узлов-опор: {0}, {1}", _anchorNode1, _anchorNode2));
        }

        private void DrawTruss(object sender, RoutedEventArgs e)
        {
            ParseTrussSettings();
            VisualiseTruss();
        }

        private void VisualiseTruss()
        {
            MainCanvas.Children.Clear();

            List<TextBlock> numbers =
                _nodes.Select(n =>
                {
                    var ixy = n.Split(' ').Select(double.Parse).ToArray();
                    SPoint p = TrussPointToCanvasPoint(new SPoint(ixy[1], ixy[2]));
                    p.X += ixy[1] < _trussLength/2 ? -15 : 15;
                    p.Y += ixy[2] == 0 ? 15 : -15;
                    return GetTextBlock(ixy[0].ToString(), p);
                }).ToList();

            List<Ellipse> ells = _truss.Select(node => GetEllipse(TrussPointToCanvasPoint(node.Point))).ToList();
            List<Path> lines = new List<Path>();

            TxtBoxJoints.Text = "";

            foreach (TrussJoint j in _truss.SelectMany(n => n.Joints).Distinct().OrderBy(j => Math.Min(j.Start.X, j.End.X)))
            {
                if (!j.Calculated)
                {
                    lines.Add(GetLine(TrussPointToCanvasPoint(j.Start), TrussPointToCanvasPoint(j.End)));
                }
                else
                {
                    double maxForce = _truss.SelectMany(n => n.Joints).Distinct().Max(joint => Math.Abs(joint.Force.Magnitude));

                    double thick = Math.Abs(j.Force.Magnitude/maxForce*5);
                    thick = thick < 1 ? 1 : thick;

                    Brush br = j.Force.Magnitude > 0 ? Brushes.Blue : Brushes.Red;

                    lines.Add(GetLine(TrussPointToCanvasPoint(j.Start), TrussPointToCanvasPoint(j.End), thick, br));

                    int n1 = _truss.ToList().IndexOf(_truss.First(n => n.Point.Equals(j.Start))) + 1;
                    int n2 = _truss.ToList().IndexOf(_truss.First(n => n.Point.Equals(j.End))) + 1;

                    TxtBoxJoints.AppendText(string.Format("Соединение {0} {1}: {2:f3} кг, {3}\r\n", n1, n2,
                        Math.Abs(j.Force.Magnitude), j.Force.Magnitude > 0 ? "растягивающее" : "сжимающее"));
                }
            }

            foreach (TrussNode n in _truss.Where(n => n.Forces.Any(f => f.Direction.Normalized.Equals(new Vector2(0, 1)))))
                MainCanvas.Children.Add(GetLine(TrussPointToCanvasPoint(n.Point),
                    TrussPointToCanvasPoint(new SPoint(n.Point.X, n.Point.Y - 2)), 4, Brushes.SaddleBrown));

            foreach (Path l in lines)
                MainCanvas.Children.Add(l);
            foreach (Ellipse el in ells)
                MainCanvas.Children.Add(el);
            foreach (TextBlock tb in numbers)
                MainCanvas.Children.Add(tb);
        }

        private void CalculateTruss(object sender, RoutedEventArgs e)
        {
            ParseTrussSettings();
            TrussCalculator.CalculateTruss(_truss);
            VisualiseTruss();
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            MainCanvas.Children.Clear();
            TxtBoxJoints.Text = "";
        }

        private void ClearTrussSetting(object sender, RoutedEventArgs e)
        {
            _nodes.Clear();
            _joints.Clear();

            UpdateHelpBox();
        }
    }
}
