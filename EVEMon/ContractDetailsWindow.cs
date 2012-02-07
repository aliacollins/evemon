using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EVEMon.Common;
using EVEMon.Common.Controls;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.API;

namespace EVEMon
{
    public sealed partial class ContractDetailsWindow : EVEMonForm
    {
        #region Fields

        private readonly Contract m_contract;
        private readonly Size m_startingSize;
        private readonly IEnumerable<ContractItem> m_contractItems;

        private ContractItemsListView m_lvIncludedItems;
        private ContractItemsListView m_lvNotIncludedItems;
        private IEnumerable<SolarSystem> m_route;
        private IEnumerable<SolarSystem> m_oldRoute;
        private Font m_smallFont;
        private Font m_boldFont;
        private Font m_mediumBoldFont;
        private Font m_bigBoldFont;
        private bool m_buttonSwitch;
        private int m_height;

        private const int Pad = 3;
        private const int FirstIntendPosition = 9;
        private const int SecondIntendPosition = 110;
        private const int ListViewHeight = 120;

        #endregion


        #region Constructors

        /// <summary>
        /// Private Constructor.
        /// </summary>
        private ContractDetailsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contract">The contract.</param>
        public ContractDetailsWindow(Contract contract)
            : this()
        {
            RememberPositionKey = "ContractDetailsWindow";
            m_startingSize = new Size(Width, Height);
            m_contract = contract;
            m_contractItems = contract.ContractItems;
        }

        #endregion


        #region Inherited Events

        /// <summary>
        /// On load, completes controls initialization.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            RoutePanelParent.Width = 160;
            RoutePanelParent.BorderStyle = BorderStyle.FixedSingle;
            RoutePanel.Size = RoutePanelParent.Size;
            Size = m_startingSize;

            // TODO: Implement a listView to show the contracts bids
            // after CCP has resolved the design defect of ContractBids API call
            //ButtonPanel.Visible = m_contract.ContractType == ContractType.Auction &&
            //                      m_contract.Character.CharacterContractBids.Any(x => x.ContractID == m_contract.ID);

            m_smallFont = FontFactory.GetDefaultFont(7.5f);
            m_boldFont = FontFactory.GetDefaultFont(FontStyle.Bold);
            m_mediumBoldFont = FontFactory.GetDefaultFont(9.25f, FontStyle.Bold);
            m_bigBoldFont = FontFactory.GetDefaultFont(10.25f, FontStyle.Bold);

            // Initialize a control for the contract's outsourcing items
            if (m_contractItems.Any(x => x.Included) && m_contractItems.Count(x => x.Included) > 1)
            {
                m_lvIncludedItems = new ContractItemsListView(m_contractItems.Where(x => x.Included))
                                        { SmallImageList = ImageListIcons };
                Controls.Add(m_lvIncludedItems);
                m_lvIncludedItems.BringToFront();
                m_lvIncludedItems.Size = new Size(DetailsPanel.Width - FirstIntendPosition * 2, 0);
            }

            // Initialize a control for the contract's incoming items
            if (m_contractItems.Any(x => !x.Included))
            {
                m_lvNotIncludedItems = new ContractItemsListView(m_contractItems.Where(x => !x.Included))
                                           { SmallImageList = ImageListIcons };
                Controls.Add(m_lvNotIncludedItems);
                m_lvNotIncludedItems.BringToFront();
            }

            // Adjust the control's height to our needs
            if (m_contract.IsTrading)
                Height += ListViewHeight;
        }


        #endregion


        #region Private Properties

        /// <summary>
        /// Gets the availability info.
        /// </summary>
        /// <value>The availability info.</value>
        private string AvailabilityInfo
        {
            get
            {
                Common.Data.Region startStationRegion = m_contract.StartStation.SolarSystem.Constellation.Region;
                Common.Data.Region characterLastKnownRegion = m_contract.Character.LastKnownSolarSystem != null
                                                                  ? m_contract.Character.LastKnownSolarSystem.Constellation.Region
                                                                  : null;
                string secondHalfText = m_contract.Availability == ContractAvailability.Private
                                            ? m_contract.Assignee
                                            : String.Format(CultureConstants.DefaultCulture, "Region: {0}  {1}",
                                                            startStationRegion.Name,
                                                            characterLastKnownRegion != null
                                                                ? characterLastKnownRegion == startStationRegion
                                                                      ? "(Current Region)"
                                                                      : "(Other Region)"
                                                                : String.Empty);

                return String.Format(CultureConstants.DefaultCulture, "{0} - {1}",
                                     m_contract.Availability.GetDescription(), secondHalfText);
            }
        }

        #endregion


        #region Details Drawing Methods

        /// <summary>
        /// Handles the Paint event of the DetailsPanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void DetailsPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw the header section
            DrawHeader(e);

            // Draw the generic info section
            DrawGenericInfo(e);

            // Draw the courier info section
            if (m_contract.ContractType == ContractType.Courier)
            {
                DrawCourierInfo(e);
                return;
            }

            // Draw the auction info section
            DrawAuctionInfo(e);

            // Draw the item exchange info section
            DrawPriceInfo(e);

            // Draw the contract's items info section
            DrawItemsInfo(e);
        }

        /// <summary>
        /// Draws the items info.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void DrawItemsInfo(PaintEventArgs e)
        {
            m_height += Pad;

            string exclamation = m_contract.Issuer == m_contract.Character.Name ? "Buyer" : "You";

            if (m_contractItems.Any(x => x.Included))
            {
                // Draw the header text
                DrawColoredText(e, String.Format(CultureConstants.DefaultCulture, "{0} Will Get", exclamation),
                                m_mediumBoldFont, new Point(FirstIntendPosition, m_height), Color.Green);

                if (m_contractItems.Count(x => x.Included) > 1)
                    DisplayListView(m_lvIncludedItems);
                else
                {
                    ContractItem contractItem = m_contractItems.First();

                    // Display the item's image
                    m_height += Pad * 2;
                    int left = DetailsPanel.Left + FirstIntendPosition + Pad * 2;
                    ItemImage.EveItem = contractItem.Item;
                    ItemImage.Location = new Point(left, m_height);
                    ItemImage.Visible = true;
                    ItemImage.BringToFront();

                    // Draw the item's name and quantity
                    m_height += Pad * 2;
                    string itemText = String.Format(CultureConstants.DefaultCulture, "{0} x {1:N0}",
                                                    contractItem.Item.Name, contractItem.Quantity);
                    Size itemTextSize = e.Graphics.MeasureString(itemText, m_bigBoldFont).ToSize();
                    int itemTextHeight = itemTextSize.Width < DetailsPanel.Width - SecondIntendPosition
                                             ? itemTextSize.Height
                                             : itemTextSize.Height * 2;
                    e.Graphics.DrawString(itemText, m_bigBoldFont, Brushes.Black,
                                          new Rectangle(left + ItemImage.Width + Pad * 2, m_height,
                                                        DetailsPanel.Width - SecondIntendPosition, itemTextHeight));

                    m_height += itemTextHeight;
                    int position = left + ItemImage.Width + Pad * 2 - DetailsPanel.Left - SecondIntendPosition;

                    // Draw the item's category and group
                    if (!String.IsNullOrEmpty(contractItem.Item.CategoryName) &&
                        !String.IsNullOrEmpty(contractItem.Item.GroupName))
                    {
                        string itemCategoryGroup = String.Format(CultureConstants.DefaultCulture, "{0}  /  {1}",
                                                                 contractItem.Item.CategoryName, contractItem.Item.GroupName);
                        DrawText(e, String.Empty, itemCategoryGroup, m_mediumBoldFont, true, position);
                    }

                    // Draw additional type info when item is a blueprint
                    if (contractItem.RawQuantity < 0 &&
                        contractItem.Item.MarketGroup.BelongsIn(DBConstants.BlueprintsMarketGroupID))
                    {
                        string itemTypeText = String.Format(CultureConstants.DefaultCulture, "BLUEPRINT {0}",
                                                            contractItem.RawQuantity == -2 ? "COPY" : "ORIGINAL");
                        DrawText(e, String.Empty, itemTypeText, m_boldFont, true, position);
                    }

                    m_height += ItemImage.Height / 2;
                }
            }

            if (m_contractItems.All(x => x.Included))
                return;

            // Draw the header text
            DrawColoredText(e, String.Format(CultureConstants.DefaultCulture, "{0} Will Give", exclamation),
                            m_mediumBoldFont, new Point(FirstIntendPosition, m_height), Color.Red);

            // Display the item's in a list view
            DisplayListView(m_lvNotIncludedItems);
        }

        /// <summary>
        /// Displays the list view.
        /// </summary>
        /// <param name="listView">The list view.</param>
        private void DisplayListView(Control listView)
        {
            m_height += Pad;
            ItemImage.Visible = !m_contract.IsBuyOnly && m_contractItems.Count(x => x.Included) == 1;
            listView.Location = new Point(FirstIntendPosition, m_height);
            int listViewHeight = m_contract.IsTrading && listView == m_lvIncludedItems
                                     ? ListViewHeight
                                     : DetailsPanel.Height - m_height - Pad * 2;
            listView.Size = new Size(DetailsPanel.Width - FirstIntendPosition * 2, listViewHeight);

            m_height += listView.Height + Pad;
        }

        /// <summary>
        /// Draws the price info.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void DrawPriceInfo(PaintEventArgs e)
        {
            if (m_contract.ContractType != ContractType.ItemExchange || (m_contract.Price == 0 && m_contract.Reward == 0))
                return;

            m_height += Pad;

            string labelText = String.Format(CultureConstants.DefaultCulture, "{0} Will {1}",
                                             m_contract.Issuer == m_contract.Character.Name ? "Buyer" : "You",
                                             m_contract.Price > 0 ? "Pay" : "Get");

            if (m_contract.Price > 0)
            {
                string priceText = String.Format(CultureConstants.DefaultCulture, GetNumberFormat(m_contract.Price),
                                                 m_contract.Price,
                                                 m_contract.Price < 10000M
                                                     ? String.Empty
                                                     : String.Format(CultureConstants.DefaultCulture, " ({0})",
                                                                     FormatHelper.Format(m_contract.Price,
                                                                                         AbbreviationFormat.AbbreviationWords,
                                                                                         false)));
                DrawText(e, labelText, String.Empty, m_mediumBoldFont, false);
                DrawColoredText(e, priceText, m_mediumBoldFont, new Point(SecondIntendPosition, m_height), Color.Red);
            }
            else
            {
                string rewardText = String.Format(CultureConstants.DefaultCulture, GetNumberFormat(m_contract.Reward),
                                                  m_contract.Reward,
                                                  m_contract.Reward < 10000M
                                                      ? String.Empty
                                                      : String.Format(CultureConstants.DefaultCulture, " ({0})",
                                                                      FormatHelper.Format(m_contract.Reward,
                                                                                          AbbreviationFormat.AbbreviationWords,
                                                                                          false)));
                DrawText(e, labelText, String.Empty, m_mediumBoldFont, false);
                DrawColoredText(e, rewardText, m_mediumBoldFont, new Point(SecondIntendPosition, m_height), Color.Green);
            }

            // Draw the lower line
            DrawLine(e);
        }

        /// <summary>
        /// Draws the auction info.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void DrawAuctionInfo(PaintEventArgs e)
        {
            if (m_contract.ContractType != ContractType.Auction)
                return;

            m_height += Pad;

            string text = String.Format(CultureConstants.DefaultCulture, GetNumberFormat(m_contract.Price),
                                        m_contract.Price,
                                        m_contract.Price < 10000M
                                            ? String.Empty
                                            : String.Format(CultureConstants.DefaultCulture, " ({0})",
                                                            FormatHelper.Format(m_contract.Price,
                                                                                AbbreviationFormat.AbbreviationWords, false)));
            DrawText(e, "Starting Bid", text, Font);

            text = m_contract.Buyout == 0
                       ? "(None)"
                       : String.Format(CultureConstants.DefaultCulture, GetNumberFormat(m_contract.Buyout),
                                       m_contract.Buyout,
                                       m_contract.Price < 10000M
                                           ? String.Empty
                                           : String.Format(CultureConstants.DefaultCulture, " ({0})",
                                                           FormatHelper.Format(m_contract.Buyout,
                                                                               AbbreviationFormat.AbbreviationWords, false)));
            DrawText(e, "Buyout Price", text, Font);

            decimal amount = m_contract.Character.CharacterContractBids.Where(x => x.ContractID == m_contract.ID).Select(
                bid => bid.Amount).Concat(new[] { 0M }).Max();
            int numberOfBids = m_contract.Character.CharacterContractBids.Count(x => x.ContractID == m_contract.ID);
            text = numberOfBids == 0
                       ? "No Bids"
                       : String.Format(CultureConstants.DefaultCulture, "{0} ({1} bid{2} so far)",
                                       String.Format(CultureConstants.DefaultCulture, GetNumberFormat(amount), amount,
                                                     String.Empty),
                                       numberOfBids, numberOfBids > 1 ? "s" : String.Empty);
            DrawText(e, "Current Bid", text, Font);

            text = m_contract.IsAvailable
                       ? m_contract.Expiration.ToRemainingTimeShortDescription(DateTimeKind.Utc)
                       : m_contract.State.ToString();

            Color color = m_contract.IsAvailable && m_contract.Expiration < DateTime.UtcNow.AddDays(1)
                              ? Color.DarkOrange
                              : m_contract.State == ContractState.Expired
                                    ? Color.Red
                                    : ForeColor;

            DrawText(e, "Time Left", String.Empty, Font, false);
            DrawColoredText(e, text, Font, new Point(SecondIntendPosition, m_height), color);

            // Draw the lower line
            DrawLine(e);
        }

        /// <summary>
        /// Draws the courier info.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void DrawCourierInfo(PaintEventArgs e)
        {
            m_height += Pad;

            DrawText(e, "Complete In", String.Format(CultureConstants.DefaultCulture, "{0} Day{1}", m_contract.DaysToComplete,
                                                     (m_contract.DaysToComplete > 1 ? "s" : String.Empty)), Font);
            DrawText(e, "Volume", String.Format(CultureConstants.DefaultCulture, "{0:N1} m³", m_contract.Volume), Font);

            string text = String.Format(CultureConstants.DefaultCulture, GetNumberFormat(m_contract.Reward), m_contract.Reward,
                                        m_contract.Reward < 10000M
                                            ? String.Empty
                                            : String.Format(CultureConstants.DefaultCulture, "({0}) ISK",
                                                            FormatHelper.Format(m_contract.Reward,
                                                                                AbbreviationFormat.AbbreviationWords, false)));
            int startToEndSystemJumps = GetNumberOfJumps(m_contract.StartStation.SolarSystem, m_contract.EndStation.SolarSystem);
            decimal iskPerJump = startToEndSystemJumps > 0 ? m_contract.Reward / startToEndSystemJumps : 0;
            string iskPerJumpText = iskPerJump > 0
                                        ? String.Format(CultureConstants.DefaultCulture, "({0} /  Jump)",
                                                        String.Format(CultureConstants.DefaultCulture, GetNumberFormat(iskPerJump),
                                                                      iskPerJump, String.Empty))
                                        : String.Empty;

            DrawText(e, "Reward", String.Empty, Font, false);
            DrawText(e, String.Empty, iskPerJumpText, Font, false, e.Graphics.MeasureString(text, Font).ToSize().Width);
            DrawColoredText(e, text, Font, new Point(SecondIntendPosition, m_height), Color.Green);

            text = String.Format(CultureConstants.DefaultCulture, GetNumberFormat(m_contract.Collateral), m_contract.Collateral,
                                 m_contract.Collateral < 10000M
                                     ? String.Empty
                                     : String.Format(CultureConstants.DefaultCulture, " ({0})",
                                                     FormatHelper.Format(m_contract.Collateral,
                                                                         AbbreviationFormat.AbbreviationWords, false)));

            DrawText(e, "Collateral", String.Empty, Font, false);
            DrawColoredText(e, text, Font, new Point(SecondIntendPosition, m_height), Color.Red);

            DrawStationText(e, "Destination", m_contract.EndStation);
        }

        /// <summary>
        /// Draws the generic contract info.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void DrawGenericInfo(PaintEventArgs e)
        {
            m_height += Pad;

            DrawText(e, "Info by Issuer", m_contract.Description, Font);
            DrawText(e, "Type", m_contract.ContractType.GetDescription(), Font);
            DrawText(e, "Issued by", m_contract.Issuer, Font);
            DrawText(e, "Availability", AvailabilityInfo, Font);
            DrawContractorText(e);
            DrawText(e, "Status", m_contract.Status.GetDescription(), Font);
            DrawStationText(e, "Location", m_contract.StartStation);
            DrawText(e, "Issued Date", m_contract.Issued.ToLocalTime().ToString("yyyy.MM.dd  HH:mm"), Font);
            DrawExpirationOrCompletionText(e);

            // Draw the lower line
            DrawLine(e);
        }

        /// <summary>
        /// Draws the expiration or completion text.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void DrawExpirationOrCompletionText(PaintEventArgs e)
        {
            // Don't display "expiration date" for auction contracts
            if (m_contract.ContractType == ContractType.Auction)
                return;

            DrawExpirationText(e);

            if (m_contract.Status != CCPContractStatus.Outstanding && m_contract.Completed != DateTime.MinValue)
                DrawText(e, "Completed Date", m_contract.Completed.ToLocalTime().ToString("yyyy.MM.dd  HH:mm"), Font);
        }

        /// <summary>
        /// Draws the expiration text.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void DrawExpirationText(PaintEventArgs e)
        {
            if (m_contract.Status != CCPContractStatus.Outstanding)
                return;

            string expirationTimeText = m_contract.Expiration.ToLocalTime().ToString("yyyy.MM.dd  HH:mm");
            string expirationRemainingTimeText = m_contract.IsAvailable
                                                     ? m_contract.Expiration.ToRemainingTimeShortDescription(DateTimeKind.Utc)
                                                     : m_contract.State.ToString();

            Size expirationTimeTextSize = e.Graphics.MeasureString(expirationTimeText, Font).ToSize();
            Size expirationRemainingTimeTextSize = e.Graphics.MeasureString(expirationRemainingTimeText, Font).ToSize();
            Color color = m_contract.IsAvailable && m_contract.Expiration < DateTime.UtcNow.AddDays(1)
                              ? Color.DarkOrange
                              : m_contract.State == ContractState.Expired
                                    ? Color.Red
                                    : ForeColor;

            int position = expirationTimeTextSize.Width;
            DrawText(e, "Expiration Date", expirationTimeText, Font, false);
            DrawText(e, String.Empty, "(", Font, false, position);
            position += Pad * 2;
            Point point = new Point(SecondIntendPosition + position, m_height);
            DrawColoredText(e, expirationRemainingTimeText, Font, point, color, false);
            position += expirationRemainingTimeTextSize.Width;
            DrawText(e, String.Empty, ")", Font, true, position);
        }

        /// <summary>
        /// Draws the contractor text.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void DrawContractorText(PaintEventArgs e)
        {
            if (m_contract.State == ContractState.Finished)
                DrawText(e, "Contractor", m_contract.Acceptor, Font);
        }

        /// <summary>
        /// Draws the station text.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        /// <param name="labelText">The label text.</param>
        /// <param name="station">The station.</param>
        private void DrawStationText(PaintEventArgs e, string labelText, Station station)
        {
            Graphics g = e.Graphics;

            string secLevelText = station.SolarSystem.SecurityLevel.ToString("N1");

            // Calculate the amount of whitespaces needed after which the station name must be drawn
            // in order to not overlap the security level text
            // (I know it looks weird but couldn't thing of another way)
            int secLevelTextWidth = g.MeasureString(secLevelText, Font).ToSize().Width;
            int spaceWidth = g.MeasureString(" ", Font).ToSize().Width;
            int tempSpaceWidth = 0;
            int spaceCount = 0;

            do
            {
                tempSpaceWidth += spaceWidth;
                spaceCount++;
            } while (secLevelTextWidth > tempSpaceWidth);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < spaceCount; i++)
            {
                sb.Append(" ");
            }
            sb.Append(station.Name);
            string stationText = sb.ToString();

            //Draw the label text
            g.DrawString(labelText, Font, Brushes.DimGray, new Point(DetailsPanel.Left + FirstIntendPosition, m_height));

            // Draw the sec level of the solar system, colored accordingly
            DrawColoredText(e, secLevelText, Font, new Point(DetailsPanel.Left + SecondIntendPosition, m_height),
                            station.SolarSystem.SecurityLevelColor, false);

            // Draw the station name
            Size stationTextSize = g.MeasureString(stationText, Font).ToSize();
            int stationTextHeight = stationTextSize.Width < DetailsPanel.Width - SecondIntendPosition
                                        ? stationTextSize.Height
                                        : stationTextSize.Height * 2;
            g.DrawString(stationText, Font, Brushes.Black, new Rectangle(DetailsPanel.Left + SecondIntendPosition, m_height,
                                                                         DetailsPanel.Width - SecondIntendPosition,
                                                                         stationTextHeight));
            m_height += stationTextHeight + Pad;

            // Draw warning text if station is a conquerable one
            if (station is ConquerableStation)
                DrawColoredText(e, "Station may be inaccesible!", Font, new Point(SecondIntendPosition, m_height), Color.DarkRed);

            // Draw the "jumps from current location to contract's start solar system" info text
            if (m_contract.Character.LastKnownSolarSystem != null && station == m_contract.StartStation)
            {
                int startToEndSystemJumps = GetNumberOfJumps(m_contract.Character.LastKnownSolarSystem, station.SolarSystem);
                string jumpsText = m_contract.Character.LastKnownStation == station
                                       ? "Current Station"
                                       : startToEndSystemJumps == 0
                                             ? "Current System"
                                             : String.Format(CultureConstants.DefaultCulture, "{0} jump{1} away - ",
                                                             startToEndSystemJumps,
                                                             startToEndSystemJumps > 1 ? "s" : String.Empty);

                Size jumpsTextSize = g.MeasureString(jumpsText, Font).ToSize();
                if (startToEndSystemJumps != 0)
                {
                    CurrentToStartLinkLabel.Location = new Point(SecondIntendPosition + jumpsTextSize.Width, m_height);
                    CurrentToStartLinkLabel.Visible = true;
                    CurrentToStartLinkLabel.BringToFront();
                }

                DrawText(e, String.Empty, jumpsText, Font);
            }

            // Draw the "jumps from current location to contract's end solar system" info text
            if (m_contract.Character.LastKnownSolarSystem != null && m_contract.StartStation != m_contract.EndStation &&
                station == m_contract.EndStation)
            {
                int startToEndSystemJumps = GetNumberOfJumps(m_contract.Character.LastKnownSolarSystem, station.SolarSystem);
                string jumpsText = m_contract.Character.LastKnownStation == station
                                       ? "Current Station"
                                       : startToEndSystemJumps == 0
                                             ? "Destination is within same solar system of start location"
                                             : String.Format(CultureConstants.DefaultCulture,
                                                             "{0} jump{1} from current location - ",
                                                             startToEndSystemJumps,
                                                             startToEndSystemJumps > 1 ? "s" : String.Empty);

                Size jumpsTextSize = g.MeasureString(jumpsText, Font).ToSize();
                if (startToEndSystemJumps != 0)
                {
                    CurrentToEndLinkLabel.Location = new Point(SecondIntendPosition + jumpsTextSize.Width, m_height);
                    CurrentToEndLinkLabel.Visible = true;
                    CurrentToEndLinkLabel.BringToFront();
                }

                DrawText(e, String.Empty, jumpsText, Font);
            }

            // Draw the "jumps between start and end solar system" info text
            if (m_contract.StartStation != m_contract.EndStation && station == m_contract.EndStation &&
                (m_contract.Character.LastKnownSolarSystem == null ||
                 (m_contract.Character.LastKnownSolarSystem != null &&
                  m_contract.Character.LastKnownSolarSystem != m_contract.StartStation.SolarSystem)))
            {
                int startToEndSystemJumps = GetNumberOfJumps(m_contract.StartStation.SolarSystem, station.SolarSystem);
                string jumpsText = startToEndSystemJumps == 0
                                       ? "Destination is within same solar system of start location"
                                       : String.Format(CultureConstants.DefaultCulture, "{0} jump{1} from start location - ",
                                                       startToEndSystemJumps, startToEndSystemJumps > 1 ? "s" : String.Empty);

                Size jumpsTextSize = g.MeasureString(jumpsText, Font).ToSize();
                if (startToEndSystemJumps != 0)
                {
                    StartToEndLinkLabel.Location = new Point(SecondIntendPosition + jumpsTextSize.Width, m_height);
                    StartToEndLinkLabel.Visible = true;
                    StartToEndLinkLabel.BringToFront();
                }

                DrawText(e, String.Empty, jumpsText, Font);
            }

            // Draw the "route through" info text
            DrawRouteThroughText(e, station);
        }

        /// <summary>
        /// Draws the "route through" text.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        /// <param name="station">The station.</param>
        private void DrawRouteThroughText(PaintEventArgs e, Station station)
        {
            bool routeThroughNullSec = false;
            bool routeThroughLowSec = false;

            if (m_contract.Character.LastKnownSolarSystem != null && station == m_contract.StartStation)
            {
                routeThroughNullSec = RouteThroughNullSec(m_contract.Character.LastKnownSolarSystem, station.SolarSystem);
                routeThroughLowSec = RouteThroughLowSec(m_contract.Character.LastKnownSolarSystem, station.SolarSystem);
            }

            if (m_contract.StartStation != m_contract.EndStation && station == m_contract.EndStation)
            {
                routeThroughNullSec = RouteThroughNullSec(m_contract.StartStation.SolarSystem, station.SolarSystem);
                routeThroughLowSec = RouteThroughLowSec(m_contract.StartStation.SolarSystem, station.SolarSystem);
            }

            // Quit if path is through high sec only
            if (!routeThroughNullSec && !routeThroughLowSec)
                return;

            Graphics g = e.Graphics;

            const string RouteText = "Route will take you through: ";
            Size routeTextSize = g.MeasureString(RouteText, Font).ToSize();

            DrawText(e, String.Empty, RouteText, Font, false);
            int left = DetailsPanel.Left + SecondIntendPosition + routeTextSize.Width;

            // Route through "Null Sec"
            if (routeThroughNullSec)
            {
                const string NullSecText = "Null Sec";
                DrawColoredText(e, NullSecText, Font, new Point(left, m_height), Color.Red, false);
                left += g.MeasureString(NullSecText, Font).ToSize().Width;
            }

            // Add comma when route through both "Null Sec" and "Low Sec"
            if (routeThroughNullSec && routeThroughLowSec)
            {
                const string CommaText = ", ";
                DrawText(e, String.Empty, CommaText, Font, false, left - Pad - DetailsPanel.Left - SecondIntendPosition);
                left += g.MeasureString(CommaText, Font).ToSize().Width;
            }

            // Route through "LowSec"
            if (routeThroughLowSec)
                DrawColoredText(e, "Low Sec", Font, new Point(left, m_height), Color.DarkOrange, false);

            m_height += routeTextSize.Height + Pad;
        }

        /// <summary>
        /// Draws a colored text.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="point">The point.</param>
        /// <param name="color">The color.</param>
        /// <param name="increaseHeight">if set to <c>true</c> [increase height].</param>
        private void DrawColoredText(PaintEventArgs e, string text, Font font, PointF point, Color color,
                                     bool increaseHeight = true)
        {
            SizeF textSize = e.Graphics.MeasureString(text, font);
            DrawColoredText(e, text, font, new RectangleF(point, textSize), color, increaseHeight);
        }

        /// <summary>
        /// Draws the colored text.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="rectangle">The rectangle.</param>
        /// <param name="color">The color.</param>
        /// <param name="increaseHeight">if set to <c>true</c> [increase height].</param>
        private void DrawColoredText(PaintEventArgs e, string text, Font font, RectangleF rectangle, Color color,
                                     bool increaseHeight = true)
        {
            Graphics g = e.Graphics;

            using (Brush secLevelBrush = new SolidBrush(color))
            {
                g.DrawString(text, font, secLevelBrush, rectangle);
            }

            if (increaseHeight)
                m_height += g.MeasureString(text, font).ToSize().Height + Pad;
        }

        /// <summary>
        /// Draws a text.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        /// <param name="labelText">The label text.</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="increaseHeight">if set to <c>true</c> [increase height].</param>
        /// <param name="position">The position.</param>
        private void DrawText(PaintEventArgs e, string labelText, string text, Font font, bool increaseHeight = true,
                              int position = 0)
        {
            Graphics g = e.Graphics;

            Size textSize = g.MeasureString(text, font).ToSize();

            // Draw the label text
            if (!String.IsNullOrEmpty(labelText))
                g.DrawString(labelText, font, Brushes.DimGray, DetailsPanel.Left + FirstIntendPosition, m_height);

            // Draw the contract's related info text
            g.DrawString(text, font, Brushes.Black, DetailsPanel.Left + SecondIntendPosition + position, m_height);

            if (increaseHeight)
                m_height += textSize.Height + Pad;
        }

        /// <summary>
        /// Draws the header.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void DrawHeader(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Image headerImage;

            // Select image according to contract type
            switch (m_contract.ContractType)
            {
                case ContractType.Courier:
                    headerImage = ImageList.Images[1];
                    break;
                case ContractType.Auction:
                    headerImage = ImageList.Images[2];
                    break;
                case ContractType.ItemExchange:
                    headerImage = ImageList.Images[0];
                    break;
                default:
                    throw new NotImplementedException();
            }

            // Draw header image
            g.DrawImage(headerImage, new Rectangle(DetailsPanel.Left, DetailsPanel.Top,
                                                   headerImage.Width, headerImage.Height));

            // Draw the header text
            Font headerTextFont = FontFactory.GetDefaultFont(10.25f);
            string headerText = String.Format(CultureConstants.DefaultCulture, "{0} ({1})", m_contract.ContractText,
                                              m_contract.ContractType.GetDescription());
            Size textSize = g.MeasureString(headerText, headerTextFont).ToSize();
            int imageWidth = headerImage.Width + Pad * 2;
            g.DrawString(headerText, headerTextFont, Brushes.Black,
                         new Rectangle(DetailsPanel.Left + imageWidth,
                                       (headerImage.Height / 2) - textSize.Height,
                                       DetailsPanel.Width - imageWidth, textSize.Height * 2));

            m_height = headerImage.Height;

            // Draw the lower line
            DrawLine(e);
        }

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void DrawLine(PaintEventArgs e)
        {
            using (Pen pen = new Pen(ForeColor))
            {
                e.Graphics.DrawLine(pen, DetailsPanel.Left + FirstIntendPosition, m_height,
                                    DetailsPanel.Right - FirstIntendPosition, m_height);
            }
        }

        #endregion


        #region Route Drawing Methods

        /// <summary>
        /// Handles the Paint event of the RoutePanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void RoutePanel_Paint(object sender, PaintEventArgs e)
        {
            if (m_route == null)
                return;

            Graphics g = e.Graphics;

            m_height = Pad;
            const string Message = "* Route is based on shortest\n  distance rather than fewer\n  gate jumps";
            SizeF messageTextSize = g.MeasureString(Message, m_smallFont);
            g.DrawString(Message, m_smallFont, Brushes.Black, Pad, m_height);
            m_height = (int)Math.Ceiling(messageTextSize.Height) + Pad * 3;
            int width = (int)Math.Ceiling(messageTextSize.Width);

            g.DrawString("Start Location:", Font, Brushes.Black, FirstIntendPosition, m_height);
            m_height += g.MeasureString("Start Location:", Font).ToSize().Height;

            width = Math.Max(width, DrawSolarSystemText(e, m_route.First(), FirstIntendPosition + Pad));
            m_height += Pad * 2;

            width = m_route.Where(solarSystem => solarSystem != m_route.First() && solarSystem != m_route.Last()).Select(
                solarSystem => DrawSolarSystemText(e, solarSystem, FirstIntendPosition * 2)).Concat(new[] { width }).Max();
            m_height += Pad * 2;

            g.DrawString("Destination:", Font, Brushes.Black, FirstIntendPosition, m_height);
            m_height += g.MeasureString("Destination:", Font).ToSize().Height;

            width = Math.Max(width, DrawSolarSystemText(e, m_route.Last(), FirstIntendPosition + Pad));

            width += Pad;
            m_height += Pad * 2 + (width > RoutePanelParent.Width ? FirstIntendPosition * 2 + Pad : Pad);

            RoutePanel.Size = new Size(width, m_height);
        }

        /// <summary>
        /// Draws the solar system text.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        /// <param name="solarSystem">The solar system.</param>
        /// <param name="left">The left.</param>
        /// <returns></returns>
        private int DrawSolarSystemText(PaintEventArgs e, SolarSystem solarSystem, int left)
        {
            Graphics g = e.Graphics;
            string secLevelText = solarSystem.SecurityLevel.ToString("N1");
            int intend = g.MeasureString(secLevelText, Font).ToSize().Width;
            Size systemTextSize = g.MeasureString(solarSystem.Name, Font).ToSize();
            DrawColoredText(e, secLevelText, Font, new Point(left, m_height),
                            solarSystem.SecurityLevelColor, false);
            g.DrawString(solarSystem.Name, Font, Brushes.Black, left + intend, m_height);

            m_height += systemTextSize.Height;

            return left + intend + systemTextSize.Width + Pad;
        }


        #endregion


        #region Local Events

        /// <summary>
        /// Handles the Click event of the DetailsPanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DetailsPanel_Click(object sender, EventArgs e)
        {
            HideRoutePanel();
        }

        /// <summary>
        /// Handles the MouseClick event of the ItemImage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ItemImage_MouseClick(object sender, MouseEventArgs e)
        {
            HideRoutePanel();
        }

        /// <summary>
        /// Handles the LinkClicked event of the CurrentToStartLinkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void CurrentToStartLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            m_route = RoutePath(m_contract.Character.LastKnownSolarSystem,
                                m_contract.StartStation.SolarSystem);
            ShowRoutePanel();
        }

        /// <summary>
        /// Handles the LinkClicked event of the CurrentToEndLinkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void CurrentToEndLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            m_route = RoutePath(m_contract.Character.LastKnownSolarSystem,
                                m_contract.EndStation.SolarSystem);
            ShowRoutePanel();
        }

        /// <summary>
        /// Handles the LinkClicked event of the StartToEndLinkLabel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
        private void StartToEndLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            m_route = RoutePath(m_contract.StartStation.SolarSystem,
                                m_contract.EndStation.SolarSystem);
            ShowRoutePanel();
        }

        /// <summary>
        /// Handles the Click event of the BidsButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void BidsButton_Click(object sender, EventArgs e)
        {
            m_buttonSwitch = !m_buttonSwitch;
            BidsButton.Text = m_buttonSwitch ? "Hide Bids" : " Show Bids";
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Shows the route panel.
        /// </summary>
        private void ShowRoutePanel()
        {
            if (RoutePanelParent.Visible)
            {
                if (!m_route.SequenceEqual(m_oldRoute))
                {
                    m_oldRoute = m_route;
                    RoutePanel.Invalidate();
                }

                return;
            }

            Width += RoutePanelParent.Width;
            RoutePanelParent.Visible = true;
            m_oldRoute = m_route;
        }

        /// <summary>
        /// Hides the route panel.
        /// </summary>
        private void HideRoutePanel()
        {
            if (!RoutePanelParent.Visible)
                return;

            RoutePanelParent.Visible = !RoutePanelParent.Visible;
            Width -= RoutePanelParent.Width;
        }


        #endregion


        #region Private Static Methods

        /// <summary>
        /// Gets the number format of an amount.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        private static string GetNumberFormat(decimal number)
        {
            return (number - (long)number == 0) ? "{0:N0} ISK {1}" : "{0:N2} ISK {1}";
        }

        /// <summary>
        /// Gets the route path between two solar systems.
        /// </summary>
        /// <param name="startSystem">The start system.</param>
        /// <param name="endSystem">The end system.</param>
        /// <returns></returns>
        private static IEnumerable<SolarSystem> RoutePath(SolarSystem startSystem, SolarSystem endSystem)
        {
            return startSystem.GetFastestPathTo(endSystem, -1);
        }

        /// <summary>
        /// Gets true if the route path between two solar systems goes through null sec.
        /// </summary>
        /// <param name="startSystem">The start system.</param>
        /// <param name="endSystem">The end system.</param>
        /// <returns></returns>
        private static bool RouteThroughNullSec(SolarSystem startSystem, SolarSystem endSystem)
        {
            return RoutePath(startSystem, endSystem).Any(x => x.IsNullSec);
        }

        /// <summary>
        /// Gets true if the route path between two solar systems goes through low sec.
        /// </summary>
        /// <param name="startSystem">The start system.</param>
        /// <param name="endSystem">The end system.</param>
        /// <returns></returns>
        private static bool RouteThroughLowSec(SolarSystem startSystem, SolarSystem endSystem)
        {
            return RoutePath(startSystem, endSystem).Any(x => x.IsLowSec);
        }

        /// <summary>
        /// Gets the number of jumps between two solar systems.
        /// </summary>
        /// <param name="startSystem">The start system.</param>
        /// <param name="endSystem">The end system.</param>
        /// <returns></returns>
        private static int GetNumberOfJumps(SolarSystem startSystem, SolarSystem endSystem)
        {
            return RoutePath(startSystem, endSystem).Count(x => x != startSystem);
        }

        #endregion


        #region Helper Class "ContractItemsListView"

        private sealed class ContractItemsListView : ListView
        {
            private readonly IEnumerable<ContractItem> m_list;
            private ColumnHeader m_sortCriteria;
            private bool m_sortAscending = true;


            #region Constructors

            /// <summary>
            /// Private Constructor.
            /// </summary>
            private ContractItemsListView()
            {
                InitializeComponent();
            }

            /// <summary>
            /// Construtor.
            /// </summary>
            /// <param name="items">The items.</param>
            internal ContractItemsListView(IEnumerable<ContractItem> items)
                : this()
            {
                m_list = items;
                m_sortCriteria = Columns[0];

                UpdateContent();
                AdjustColumns();
            }

            #endregion


            #region Initializer

            /// <summary>
            /// Initializes the component.
            /// </summary>
            private void InitializeComponent()
            {
                ColumnClick += ContractItemsListView_ColumnClick;
                View = View.Details;
                FullRowSelect = true;
                HideSelection = false;
                Columns.AddRange(new[]
                                     {
                                         new ColumnHeader { Text = "Name", Width = 120 },
                                         new ColumnHeader
                                             {
                                                 Text = "Qty",
                                                 Width = 60,
                                                 TextAlign = HorizontalAlignment.Right
                                             },
                                         new ColumnHeader { Text = "Type", Width = 60 }
                                     });
            }

            #endregion


            #region Updating Methods

            /// <summary>
            /// Updates the content.
            /// </summary>
            private void UpdateContent()
            {
                BeginUpdate();
                try
                {
                    UpdateSort();

                    Items.Clear();

                    foreach (ContractItem contractItem in m_list)
                    {
                        ListViewItem lvItem = new ListViewItem(contractItem.Item.Name) { Tag = contractItem };

                        // Add enough subitems to match the number of columns
                        while (lvItem.SubItems.Count < Columns.Count + 1)
                        {
                            lvItem.SubItems.Add(String.Empty);
                        }

                        // Creates the subitems
                        for (int i = 0; i < Columns.Count; i++)
                        {
                            SetColumn(contractItem, lvItem.SubItems[i], Columns[i]);
                        }
                        Items.Add(lvItem);
                    }
                }
                finally
                {
                    EndUpdate();
                }
            }

            /// <summary>
            /// Adjusts the columns.
            /// </summary>
            private void AdjustColumns()
            {
                foreach (ColumnHeader column in Columns.Cast<ColumnHeader>())
                {
                    column.Width = -2;

                    // Due to .NET design we need to prevent the last colummn to resize to the right end

                    // Return if it's not the last column and not set to auto-resize
                    if (column.Index != Columns.Count - 1)
                        continue;

                    const int Pad = 4;

                    // Calculate column header text width with padding
                    int columnHeaderWidth = TextRenderer.MeasureText(column.Text, Font).Width + Pad * 2;

                    // If there is an image assigned to the header, add its width with padding
                    if (SmallImageList != null && column.ImageIndex > -1)
                        columnHeaderWidth += SmallImageList.ImageSize.Width + Pad;

                    // Calculate the width of the header and the items of the column
                    int columnMaxWidth = column.ListView.Items.Cast<ListViewItem>().Select(
                        item => TextRenderer.MeasureText(item.SubItems[column.Index].Text, Font).Width).Concat(
                            new[] { columnHeaderWidth }).Max() + Pad + 1;

                    // Assign the width found
                    column.Width = columnMaxWidth;
                }
            }

            /// <summary>
            /// Updates the item sorter.
            /// </summary>
            private void UpdateSort()
            {
                ListViewItemSorter = new ListViewItemComparerByTag<ContractItem>(
                    new ContractItemComparer(m_sortCriteria, m_sortAscending));

                UpdateSortVisualFeedback();
            }

            /// <summary>
            /// Updates the sort feedback (the arrow on the header).
            /// </summary>
            private void UpdateSortVisualFeedback()
            {
                foreach (ColumnHeader columnHeader in Columns.Cast<ColumnHeader>())
                {
                    if (m_sortCriteria == columnHeader)
                        columnHeader.ImageIndex = (m_sortAscending ? 0 : 1);
                    else
                        columnHeader.ImageIndex = 2;
                }
            }

            /// <summary>
            /// Updates the listview sub-item.
            /// </summary>
            /// <param name="contractItem">The contract item.</param>
            /// <param name="item">The item.</param>
            /// <param name="column">The column.</param>
            /// <exception cref="NotImplementedException"></exception>
            private static void SetColumn(ContractItem contractItem, ListViewItem.ListViewSubItem item, ColumnHeader column)
            {
                switch (column.Index)
                {
                    case 0:
                        item.Text = contractItem.Item.Name;
                        break;
                    case 1:
                        item.Text = contractItem.Quantity.ToString(CultureConstants.DefaultCulture);
                        break;
                    case 2:
                        item.Text = contractItem.Item.GroupName;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }


            #region Local Events

            /// <summary>
            /// When the user clicks a column header, we update the sorting.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void ContractItemsListView_ColumnClick(object sender, ColumnClickEventArgs e)
            {
                ListView lvItems = (ListView)sender;
                ColumnHeader column = lvItems.Columns[e.Column];
                if (m_sortCriteria == column)
                    m_sortAscending = !m_sortAscending;
                else
                {
                    m_sortCriteria = column;
                    m_sortAscending = true;
                }

                UpdateContent();
            }

            #endregion
        }

        #endregion


        #endregion
    }
}
