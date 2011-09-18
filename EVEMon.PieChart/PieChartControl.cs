using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace EVEMon.PieChart
{
    /// <summary>
    /// Summary description for PieChartControl.
    /// </summary>
    public class PieChartControl : Panel
    {
        private float m_leftMargin;
        private float m_topMargin;
        private float m_rightMargin;
        private float m_bottomMargin;
        private bool m_fitChart;

        private decimal[] m_values;
        private Color[] m_colors;
        private float m_sliceRelativeHeight;
        private float[] m_relativeSliceDisplacements = new[] { 0F };
        private ShadowStyle m_shadowStyle = ShadowStyle.GradualShadow;
        private EdgeColorType m_edgeColorType = EdgeColorType.SystemColor;
        private float m_edgeLineWidth = 1F;
        private float m_initialAngle;
        private int m_highlightedIndex = -1;
        private readonly ToolTip m_toolTip;

        private int m_lastX = -1;
        private int m_lastY = -1;

        // These are used for the actual drawing. They are modified depending
        // on wether sorting by size is on or off
        private decimal[] m_drawValues;
        private Color[] m_drawColors;
        private float[] m_drawRelativeSliceDisplacements = new[] { 0F };
        private string[] m_drawToolTipTexts;
        private string[] m_drawTexts;
        private int[] m_sortOrder;

        /// <summary>
        ///   Default AutoPopDelay of the ToolTip control.
        /// </summary>
        private int m_defaultToolTipAutoPopDelay;

        /// <summary>
        ///   Flag indicating that object has been disposed.
        /// </summary>
        private bool m_disposed;

        private bool m_mouseDown;

        /// <summary>
        ///   Initializes the <c>PieChartControl</c>.
        /// </summary>
        public PieChartControl()
        {
            Texts = null;
            ToolTips = null;
            PieChart = null;
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
            m_toolTip = new ToolTip();
        }

        public PieChart3D PieChart { get; private set; }

        /// <summary>
        ///   Sets the left margin for the chart.
        /// </summary>
        public float LeftMargin
        {
            set
            {
                Debug.Assert(value >= 0);
                m_leftMargin = value;
                Invalidate();
            }
        }

        /// <summary>
        ///   Sets the right margin for the chart.
        /// </summary>
        public float RightMargin
        {
            set
            {
                Debug.Assert(value >= 0);
                m_rightMargin = value;
                Invalidate();
            }
        }

        /// <summary>
        ///   Sets the top margin for the chart.
        /// </summary>
        public float TopMargin
        {
            set
            {
                Debug.Assert(value >= 0);
                m_topMargin = value;
                Invalidate();
            }
        }

        /// <summary>
        ///   Sets the bottom margin for the chart.
        /// </summary>
        public float BottomMargin
        {
            set
            {
                Debug.Assert(value >= 0);
                m_bottomMargin = value;
                Invalidate();
            }
        }

        /// <summary>
        ///   Sets the indicator if chart should fit the bounding rectangle
        ///   exactly.
        /// </summary>
        public bool FitChart
        {
            set
            {
                m_fitChart = value;
                Invalidate();
            }
        }

        /// <summary>
        ///   Sets values to be represented by the chart.
        /// </summary>
        public decimal[] Values
        {
            set
            {
                m_values = value;
                Invalidate();
            }
        }

        /// <summary>
        ///   Sets colors to be used for rendering pie slices.
        /// </summary>
        public Color[] Colors
        {
            set
            {
                m_colors = value;
                Invalidate();
            }
            get { return m_colors; }
        }

        /// <summary>
        ///   Sets values for slice displacements.
        /// </summary>
        public float[] SliceRelativeDisplacements
        {
            set
            {
                m_relativeSliceDisplacements = value;
                Invalidate();
            }
        }

        /// <summary>
        ///   Gets or sets tooltip texts.
        /// </summary>
        public string[] ToolTips { private get; set; }

        /// <summary>
        ///   Sets texts appearing by each pie slice.
        /// </summary>
        public string[] Texts { private get; set; }

        /// <summary>
        ///   Sets pie slice reative height.
        /// </summary>
        public float SliceRelativeHeight
        {
            set
            {
                m_sliceRelativeHeight = value;
                Invalidate();
            }
        }

        /// <summary>
        ///   Sets the shadow style.
        /// </summary>
        public ShadowStyle ShadowStyle
        {
            set
            {
                m_shadowStyle = value;
                Invalidate();
            }
        }

        /// <summary>
        ///  Sets the edge color type.
        /// </summary>
        public EdgeColorType EdgeColorType
        {
            set
            {
                m_edgeColorType = value;
                Invalidate();
            }
        }

        /// <summary>
        ///   Sets the edge lines width.
        /// </summary>
        public float EdgeLineWidth
        {
            set
            {
                m_edgeLineWidth = value;
                Invalidate();
            }
        }

        /// <summary>
        ///   Sets the initial angle from which pies are drawn.
        /// </summary>
        public float InitialAngle
        {
            set
            {
                float newAngle = value;

                if (newAngle > 360.0f)
                    newAngle -= 360.0f;

                if (newAngle < 0.0f)
                    newAngle += 360.0f;

                OnAngleChange(new AngleChangeEventArgs(m_initialAngle, newAngle));

                m_initialAngle = newAngle;
                Invalidate();
            }
        }

        /// <summary>
        ///   Handles <c>OnPaint</c> event.
        /// </summary>
        /// <param name="args">
        ///   <c>PaintEventArgs</c> object.
        /// </param>
        protected override void OnPaint(PaintEventArgs args)
        {
            base.OnPaint(args);
            if (HasAnyValue)
                DoDraw(args.Graphics);
        }

        /// <summary>
        ///   Sets values for the chart and draws them.
        /// </summary>
        /// <param name="graphics">
        ///   Graphics object used for drawing.
        /// </param>
        private void DoDraw(Graphics graphics)
        {
            if (m_drawValues == null || m_drawValues.Length <= 0)
                return;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            float width = ClientSize.Width - m_leftMargin - m_rightMargin;
            float height = ClientSize.Height - m_topMargin - m_bottomMargin;

            // if the width or height if <=0 an exception would be thrown -> exit method..
            if (width <= 0 || height <= 0)
                return;

            if (PieChart != null)
                PieChart.Dispose();

            if (m_drawColors != null && m_drawColors.Length > 0)
            {
                PieChart = new PieChart3D(m_leftMargin, m_topMargin, width, height, m_drawValues, m_drawColors,
                                          m_sliceRelativeHeight, m_drawTexts);
            }
            else
            {
                PieChart = new PieChart3D(m_leftMargin, m_topMargin, width, height, m_drawValues, m_sliceRelativeHeight,
                                          m_drawTexts);
            }
            PieChart.FitToBoundingRectangle = m_fitChart;
            PieChart.InitialAngle = m_initialAngle;
            PieChart.SliceRelativeDisplacements = m_drawRelativeSliceDisplacements;
            PieChart.EdgeColorType = m_edgeColorType;
            PieChart.EdgeLineWidth = m_edgeLineWidth;
            PieChart.ShadowStyle = m_shadowStyle;
            PieChart.HighlightedIndex = m_highlightedIndex;
            PieChart.Draw(graphics);
            PieChart.Font = Font;
            PieChart.ForeColor = ForeColor;
            PieChart.PlaceTexts(graphics);
        }

        /// <summary>
        ///   Handles <c>MouseEnter</c> event to activate the tooltip.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            m_defaultToolTipAutoPopDelay = m_toolTip.AutoPopDelay;
            m_toolTip.AutoPopDelay = Int16.MaxValue;
        }

        /// <summary>
        ///   Handles <c>MouseLeave</c> event to disable tooltip.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            m_toolTip.RemoveAll();
            m_toolTip.AutoPopDelay = m_defaultToolTipAutoPopDelay;
            m_highlightedIndex = -1;
            Refresh();
        }

        /// <summary>
        /// Handles <c>MouseDown</c> event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            m_mouseDown = true;
        }

        /// <summary>
        /// Handles <c>MouseUp</c> event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            m_mouseDown = false;
        }

        /// <summary>
        ///   Handles <c>MouseMove</c> event to display tooltip for the pie
        ///   slice under pointer and to display slice in highlighted color.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (PieChart == null || m_values == null || m_values.Length <= 0)
                return;

            if (e.X == m_lastX && e.Y == m_lastY)
                return;

            if (m_mouseDown)
                InitialAngle = m_initialAngle - (e.X - m_lastX);
            else
            {
                int index = PieChart.FindPieSliceUnderPoint(new PointF(e.X, e.Y));

                if (index != m_highlightedIndex)
                {
                    m_highlightedIndex = index;
                    Refresh();
                }

                if (m_highlightedIndex != -1)
                {
                    if (m_drawToolTipTexts == null || m_drawToolTipTexts.Length <= m_highlightedIndex ||
                        m_drawToolTipTexts[m_highlightedIndex].Length == 0)
                        m_toolTip.SetToolTip(this, m_values[m_highlightedIndex].ToString());
                    else
                        m_toolTip.SetToolTip(this, m_drawToolTipTexts[m_highlightedIndex]);
                }
                else
                    m_toolTip.RemoveAll();
            }

            m_lastX = e.X;
            m_lastY = e.Y;
        }

        /// <summary>
        ///   Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (m_disposed)
                return;

            try
            {
                if (disposing)
                {
                    PieChart.Dispose();
                    m_toolTip.Dispose();
                }
                m_disposed = true;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        ///   Gets a flag indicating if at least one value is nonzero.
        /// </summary>
        private bool HasAnyValue
        {
            get { return m_values != null && m_values.Any(angle => angle != 0); }
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <param name="sliceIndex">Index of the slice.</param>
        /// <returns></returns>
        public int GetIndex(int sliceIndex)
        {
            return m_sortOrder[sliceIndex];
        }

        /// <summary>
        /// Event for when the graph angle changes.
        /// </summary>
        public event EventHandler AngleChange;

        /// <summary>
        /// Event for when the graph angle changes
        /// </summary>
        /// <param name="e"></param>
        private void OnAngleChange(AngleChangeEventArgs e)
        {
            if (AngleChange != null)
                AngleChange(this, e);
        }

        /// <summary>
        /// Will copy the original data to the vars used for drawing.
        /// The original is needed to use for ordering.
        /// </summary>
        private void CopyDataToDrawVars()
        {
            m_drawValues = (decimal[])m_values.Clone();
            m_drawColors = (Color[])m_colors.Clone();
            m_drawRelativeSliceDisplacements = (float[])m_relativeSliceDisplacements.Clone();
            m_drawToolTipTexts = (string[])ToolTips.Clone();
            m_drawTexts = (string[])Texts.Clone();


            // fill the sort order to default:
            m_sortOrder = new int[m_values.Length];
            for (int i = 0; i < m_values.Length; i++)
            {
                m_sortOrder[i] = i;
            }
        }

        /// <summary>
        /// Orders the slices.
        /// </summary>
        /// <param name="orderBySize">if set to <c>true</c> [order by size].</param>
        public void OrderSlices(bool orderBySize)
        {
            if (orderBySize && m_values != null)
            {
                // prefill the draw vars
                CopyDataToDrawVars();

                // take a copy of the original values
                // then use it to do the calculations
                decimal[] values = (decimal[])m_values.Clone();
                Color[] colours = (Color[])m_colors.Clone();
                float[] displacements = (float[])m_relativeSliceDisplacements.Clone();
                string[] tooltips = (string[])ToolTips.Clone();
                string[] texts = (string[])Texts.Clone();

                // reordering the slices
                for (int num = 0; num < values.Length; num++)
                {
                    decimal tempsp = decimal.MinValue;
                    int biggest = -1;
                    for (int y = 0; y < values.Length; y++)
                    {
                        if (biggest == -1)
                        {
                            tempsp = values[y];
                            biggest = y;
                        }
                        if (values[y] <= tempsp || values[y] <= 0)
                            continue;

                        tempsp = values[y];
                        biggest = y;
                    }

                    m_drawValues[num] = values[biggest];
                    m_drawTexts[num] = texts[biggest];
                    m_drawRelativeSliceDisplacements[num] = displacements[biggest];
                    m_drawToolTipTexts[num] = tooltips[biggest];
                    m_drawColors[num] = colours[biggest];
                    m_sortOrder[num] = biggest;
                    values[biggest] = 0;
                }
            }
            else
            {
                if (m_values != null)
                    CopyDataToDrawVars();
            }

            Refresh();
        }
    }
}