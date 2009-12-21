using System;
using System.Collections;
using System.Windows.Forms;
using EVEMon.Common;

namespace EVEMon.SkillPlanner
{
    public class PersistentSplitContainer : SplitContainer
    {
        public PersistentSplitContainer()
            : base()
        {
        }
        
        private string m_rememberDistanceKey;

        public string RememberDistanceKey
        {
            get { return m_rememberDistanceKey; }
            set 
            { 
                m_rememberDistanceKey = value;
                // Set the splitter width here rather than in an override of CreateControl()
                // because CreatControl is only called when we make the container visible
                // so if the container is created, but never shown, the persistant splitter 
                // width will be reset to the default for the base SplitContainer
                try
                {
                    if (!String.IsNullOrEmpty(m_rememberDistanceKey))
                    {
                        Settings s = Settings.GetInstance();
                        if (s.SavedSplitterDistances.ContainsKey(m_rememberDistanceKey))
                        {
                            int d = s.SavedSplitterDistances[m_rememberDistanceKey];
                            d = this.VerifyValidSplitterDistance(d);
                            this.SplitterDistance = d;
                        }
                    }
                }
                catch (Exception err)
                {
                    // This occurs when we're in the designer. DesignMode doesn't get set
                    // when the control is a subcontrol of a user control, so we should handle
                    // this here :(
                    ExceptionHandler.LogException(err, true);
                    return;
                }

            }
        }

       protected override void Dispose(bool disposing)
        {
            if (!String.IsNullOrEmpty(m_rememberDistanceKey))
            {
                Settings s = Settings.GetInstance();
                int d = this.SplitterDistance;
                if (VerifyValidSplitterDistance(d) == d)
                {
                    s.SavedSplitterDistances[m_rememberDistanceKey] = d;
                }
            }

            base.Dispose(disposing);
        }

        private int VerifyValidSplitterDistance(int d)
        {
            int defaultDistance = this.Width / 4;

            if ((d < this.Panel1MinSize) || (d + this.Panel2MinSize > this.Width))
                return defaultDistance;
            else
                return d;
        }
    }
}