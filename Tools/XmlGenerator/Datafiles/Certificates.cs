using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EVEMon.Common;
using EVEMon.Common.Data;
using EVEMon.Common.Serialization.Datafiles;
using EVEMon.XmlGenerator.StaticData;

namespace EVEMon.XmlGenerator.Datafiles
{
    public static class Certificates
    {
        /// <summary>
        /// Generate the certificates datafile.
        /// </summary>        
        internal static void GenerateDatafile()
        {
            DateTime startTime = DateTime.Now;
            Util.ResetCounters();

            Console.WriteLine();
            Console.Write("Generating certificates datafile... ");

            // Export certificates categories
            List<SerializableCertificateCategory> listOfCertCategories = new List<SerializableCertificateCategory>();

            foreach (CrtCategories category in Database.CrtCategoriesTable.OrderBy(x => x.CategoryName))
            {
                SerializableCertificateCategory crtCategory = new SerializableCertificateCategory
                                                                  {
                                                                      ID = category.ID,
                                                                      Name = category.CategoryName,
                                                                      Description = category.Description
                                                                  };

                // Add classes to categories
                crtCategory.Classes.AddRange(ExportCertificateClasses(category));

                // Add category
                listOfCertCategories.Add(crtCategory);
            }

            // Serialize
            CertificatesDatafile datafile = new CertificatesDatafile();
            datafile.Categories.AddRange(listOfCertCategories);

            Console.WriteLine(String.Format(CultureConstants.DefaultCulture, " in {0}",
                                            DateTime.Now.Subtract(startTime)).TrimEnd('0'));

            Util.SerializeXML(datafile, DatafileConstants.CertificatesDatafile);
        }

        /// <summary>
        /// Exports the certificates.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        private static IEnumerable<SerializableCertificateClass> ExportCertificateClasses(IHasID category)
        {
            List<SerializableCertificateClass> listOfCertClasses = new List<SerializableCertificateClass>();

            int categoryID = 0;
            foreach (CrtClasses certClass in Database.CrtClassesTable)
            {
                // Exclude unused classes
                int id = certClass.ID;
                if (id == DBConstants.IndustrialHarvestingID ||
                    id == DBConstants.AutomatedMiningID ||
                    id == DBConstants.ProductionInternID)
                    continue;

                SerializableCertificateClass crtClasses = new SerializableCertificateClass
                                                              {
                                                                  ID = certClass.ID,
                                                                  Name = certClass.ClassName,
                                                                  Description = certClass.Description
                                                              };

                // Export certificates
                List<SerializableCertificate> listOfCertificates = new List<SerializableCertificate>();

                foreach (CrtCertificates certificate in Database.CrtCertificatesTable.Where(x => x.ClassID == certClass.ID))
                {

                    // Storing the certificate categoryID for use in classes
                    categoryID = certificate.CategoryID;

                    ExportCertificate(listOfCertificates, certificate);
                }

                // Grouping certificates according to their classes
                if (categoryID != category.ID)
                    continue;

                // Add certificates to classes
                crtClasses.Certificates.AddRange(listOfCertificates.OrderBy(x => x.Grade));

                // Add certificate class
                listOfCertClasses.Add(crtClasses);
            }
            return listOfCertClasses;
        }

        /// <summary>
        /// Exports the certificate.
        /// </summary>
        /// <param name="listOfCertificates">The list of certificates.</param>
        /// <param name="certificate">The certificate.</param>
        private static void ExportCertificate(ICollection<SerializableCertificate> listOfCertificates, CrtCertificates certificate)
        {
            Util.UpdatePercentDone(Database.CertificatesTotalCount);

            SerializableCertificate crtCertificates = new SerializableCertificate
                                                          {
                                                              ID = certificate.ID,
                                                              Grade = GetGrade(certificate.Grade),
                                                              Description = certificate.Description
                                                          };

            // Export prerequesities
            List<SerializableCertificatePrerequisite> listOfPrereq = new List<SerializableCertificatePrerequisite>();

            foreach (CrtRelationships relationship in Database.CrtRelationshipsTable.Where(x => x.ChildID == certificate.ID))
            {
                SerializableCertificatePrerequisite crtPrerequisites = new SerializableCertificatePrerequisite
                                                                           {
                                                                               ID = relationship.ID,
                                                                           };


                if (relationship.ParentTypeID != null) // prereq is a skill
                {
                    InvTypes skill = Database.InvTypesTable.First(x => x.ID == relationship.ParentTypeID);
                    crtPrerequisites.Kind = SerializableCertificatePrerequisiteKind.Skill;
                    crtPrerequisites.Name = skill.Name;
                    crtPrerequisites.Level = relationship.ParentLevel.ToString();
                }
                else // prereq is a certificate
                {
                    CrtCertificates cert = Database.CrtCertificatesTable.First(x => x.ID == relationship.ParentID);
                    CrtClasses crtClass = Database.CrtClassesTable.First(x => x.ID == cert.ClassID);
                    crtPrerequisites.Kind = SerializableCertificatePrerequisiteKind.Certificate;
                    crtPrerequisites.Name = crtClass.ClassName;
                    crtPrerequisites.Level = GetGrade(cert.Grade).ToString();
                }

                // Add prerequisite
                listOfPrereq.Add(crtPrerequisites);
            }

            //Add prerequisites to certificate
            crtCertificates.Prerequisites.AddRange(listOfPrereq);

            // Add recommendations to certificate
            IEnumerable<SerializableCertificateRecommendation> listOfRecommendations = Database.CrtRecommendationsTable.Where(
                x => x.CertificateID == certificate.ID).Select(
                    recommendation =>
                    new
                        {
                            recommendation,
                            shipName = Database.InvTypesTable.First(x => x.ID == recommendation.ShipTypeID)
                        }).Select(certRecom => new SerializableCertificateRecommendation
                                                   {
                                                       ID = certRecom.recommendation.ID,
                                                       Ship = certRecom.shipName.Name,
                                                       Level = certRecom.recommendation.Level
                                                   });

            crtCertificates.Recommendations.AddRange(listOfRecommendations);

            // Add certificate
            listOfCertificates.Add(crtCertificates);
        }

        /// <summary>
        /// Gets the certificates Grade.
        /// </summary>        
        private static CertificateGrade GetGrade(int gradeValue)
        {
            switch (gradeValue)
            {
                case DBConstants.BasicID:
                    return CertificateGrade.Basic;
                case DBConstants.StandardID:
                    return CertificateGrade.Standard;
                case DBConstants.ImprovedID:
                    return CertificateGrade.Improved;
                case DBConstants.EliteID:
                    return CertificateGrade.Elite;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
