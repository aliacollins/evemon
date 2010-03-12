using System;
using System.Collections.Generic;
using System.Text;
using EVEMon.XmlGenerator.StaticData;

namespace EVEMon.XmlGenerator
{
    public static class Database
    {
        private static EveStaticDataEntities s_entities;

        /// <summary>
        /// Makes the context available to all database methods without
        /// reinstantiating each time.
        /// </summary>
        private static EveStaticDataEntities context
        {
            get
            {
                if (s_entities == null)
                    s_entities = new EveStaticDataEntities();

                return s_entities;
            }
        }

        /// <summary>
        /// EVE Units
        /// </summary>
        /// <returns>Bag of EVE Units</returns>
        internal static Bag<EveUnit> Units()
        {
            IndexedList<EveUnit> list = new IndexedList<EveUnit>();

            foreach (var unit in context.eveUnits)
            {
                EveUnit item = new EveUnit()
                {
                    Description = unit.description.Clean(),
                    DisplayName = unit.displayName.Clean(),
                    ID = unit.unitID,
                    Name = unit.unitName
                };

                list.Items.Add(item);
            }

            return new Bag<EveUnit>(list);
        }

        internal static Bag<EveGraphic> Graphics()
        {
            IndexedList<EveGraphic> list = new IndexedList<EveGraphic>();

            foreach (var graphic in context.eveGraphics)
            {
                EveGraphic item = new EveGraphic()
                {
                    Icon = graphic.icon,
                    ID = graphic.graphicID
                };

                list.Items.Add(item);
            }

            return new Bag<EveGraphic>(list);
        }

        internal static Bag<DgmAttribute> Attributes()
        {
            IndexedList<DgmAttribute> list = new IndexedList<DgmAttribute>();

            foreach (var attribute in context.dgmAttributeTypes)
            {
                DgmAttribute item = new DgmAttribute()
                {
                    ID = attribute.attributeID,
                    CategoryID = attribute.categoryID,
                    Description = attribute.description.Clean(),
                    DisplayName = attribute.displayName.Clean(),
                    GraphicID = attribute.graphicID,
                    Name = attribute.attributeName.Clean(),
                    UnitID = attribute.unitID
                };

                if (attribute.defaultValue.HasValue)
                    item.DefaultValue = attribute.defaultValue.Value.ToString();

                if (attribute.highIsGood.HasValue)
                    item.HigherIsBetter = attribute.highIsGood.Value;

                list.Items.Add(item);
            }

            return new Bag<DgmAttribute>(list);
        }

        internal static Bag<DgmAttributeCategory> AttributeCategories()
        {
            IndexedList<DgmAttributeCategory> list = new IndexedList<DgmAttributeCategory>();

            foreach (var category in context.dgmAttributeCategories)
            {
                DgmAttributeCategory item = new DgmAttributeCategory()
                {
                    ID = category.categoryID,
                    Description = category.categoryDescription.Clean(),
                    Name = category.categoryName.Clean()
                };

                list.Items.Add(item);
            }

            return new Bag<DgmAttributeCategory>(list);
        }

        internal static Bag<MapRegion> Regions()
        {
            IndexedList<MapRegion> list = new IndexedList<MapRegion>();

            foreach (var region in context.mapRegions)
            {
                MapRegion item = new MapRegion()
                {
                    ID = region.regionID,
                    Name = region.regionName,
                    FactionID = region.factionID
                };

                list.Items.Add(item);
            }

            return new Bag<MapRegion>(list);
        }

        internal static Bag<MapConstellation> Constellations()
        {
            IndexedList<MapConstellation> list = new IndexedList<MapConstellation>();

            foreach (var constellation in context.mapConstellations)
            {
                MapConstellation item = new MapConstellation()
                {
                    ID = constellation.constellationID,
                    Name = constellation.constellationName,
                };

                if (constellation.regionID.HasValue)
                    item.RegionID = constellation.regionID.Value;

                list.Items.Add(item);
            }

            return new Bag<MapConstellation>(list);
        }

        internal static Bag<MapSolarSystem> Solarsystems()
        {
            IndexedList<MapSolarSystem> list = new IndexedList<MapSolarSystem>();

            foreach (var solarsystem in context.mapSolarSystems)
            {
                MapSolarSystem item = new MapSolarSystem()
                {
                    ID = solarsystem.solarSystemID,
                    Name = solarsystem.solarSystemName
                };

                if (solarsystem.constellationID.HasValue)
                    item.ConstellationID = solarsystem.constellationID.Value;

                if (solarsystem.security.HasValue)
                    item.SecurityLevel = (float)solarsystem.security.Value;

                if (solarsystem.x.HasValue)
                    item.X = solarsystem.x.Value;

                if (solarsystem.y.HasValue)
                    item.Y = solarsystem.y.Value;

                if (solarsystem.z.HasValue)
                    item.Z = solarsystem.z.Value;

                list.Items.Add(item);
            }

            return new Bag<MapSolarSystem>(list);
        }

        internal static Bag<StaStation> Stations()
        {
            IndexedList<StaStation> list = new IndexedList<StaStation>();

            foreach (var station in context.staStations)
            {
                StaStation item = new StaStation()
                {
                    ID = station.stationID,
                    Name = station.stationName,
                };

                if (station.reprocessingEfficiency.HasValue)
                    item.ReprocessingEfficiency = (float)station.reprocessingEfficiency.Value;

                if (station.reprocessingStationsTake.HasValue)
                    item.ReprocessingStationsTake = (float)station.reprocessingStationsTake.Value;

                if (station.security.HasValue)
                    item.SecurityLevel = station.security.Value;

                if (station.solarSystemID.HasValue)
                    item.SolarSystemID = station.solarSystemID.Value;

                list.Items.Add(item);
            }

            return new Bag<StaStation>(list);
        }

        internal static List<MapSolarSystemJump> Jumps()
        {
            List<MapSolarSystemJump> list = new List<MapSolarSystemJump>();

            foreach (var jump in context.mapSolarSystemJumps)
            {
                MapSolarSystemJump item = new MapSolarSystemJump()
                {
                    A = jump.fromSolarSystemID,
                    B = jump.toSolarSystemID
                };

                list.Add(item);
            }

            return list;
        }

        internal static Bag<InvMarketGroup> MarketGroups()
        {
            IndexedList<InvMarketGroup> list = new IndexedList<InvMarketGroup>();

            foreach (var marketGroup in context.invMarketGroups)
            {
                InvMarketGroup item = new InvMarketGroup()
                {
                    ID = marketGroup.marketGroupID,
                    Description = marketGroup.description.Clean(),
                    GraphicID = marketGroup.graphicID,
                    Name = marketGroup.marketGroupName,
                    ParentID = marketGroup.parentGroupID
                };

                list.Items.Add(item);
            }

            return new Bag<InvMarketGroup>(list);
        }

        internal static Bag<InvGroup> Groups()
        {
            IndexedList<InvGroup> list = new IndexedList<InvGroup>();

            foreach (var group in context.invGroups)
            {
                InvGroup item = new InvGroup()
                {
                    ID = group.groupID,
                    Name = group.groupName
                };

                if (group.categoryID.HasValue)
                    item.CategoryID = group.categoryID.Value;

                list.Items.Add(item);
            }

            return new Bag<InvGroup>(list);
        }

        internal static Bag<InvType> Types()
        {
            IndexedList<InvType> list = new IndexedList<InvType>();

            foreach (var type in context.invTypes)
            {
                InvType item = new InvType()
                {
                    ID = type.typeID,
                    Description = type.description.Clean(),
                    GraphicID = type.graphicID,
                    MarketGroupID = type.marketGroupID,
                    Name = type.typeName,
                    RaceID = type.raceID
                };

                if (type.basePrice.HasValue)
                    item.BasePrice = type.basePrice.Value;

                if (type.capacity.HasValue)
                    item.Capacity = type.capacity.Value;

                if (type.groupID.HasValue)
                    item.GroupID = type.groupID.Value;

                if (type.mass.HasValue)
                    item.Mass = type.mass.Value;

                if (type.published.HasValue)
                    item.Published = type.published.Value;

                if (type.volume.HasValue)
                    item.Volume = type.volume.Value;

                list.Items.Add(item);
            }

            return new Bag<InvType>(list);
        }

        internal static List<TypeActivityMaterial> Materials()
        {
            List<TypeActivityMaterial> list = new List<TypeActivityMaterial>();

            foreach (var material in context.ramTypeRequirements)
            {
                TypeActivityMaterial item = new TypeActivityMaterial()
                {
                    ActivityID = material.activityID,
                    RequiredTypeID = material.requiredTypeID,
                    TypeID = material.typeID
                };

                if (material.quantity.HasValue)
                    item.Quantity = material.quantity.Value;

                list.Add(item);
            }

            return list;
        }

        internal static Bag<CrtCategories> CertificateCategories()
        {
            IndexedList<CrtCategories> list = new IndexedList<CrtCategories>();

            foreach (var category in context.crtCategories)
            {
                CrtCategories item = new CrtCategories()
                {
                    ID = category.categoryID,
                    CategoryName = category.categoryName,
                    Description = category.description.Clean()
                };

                list.Items.Add(item);
            }

            return new Bag<CrtCategories>(list);
        }

        internal static Bag<CrtClasses> CertificateClasses()
        {
            IndexedList<CrtClasses> list = new IndexedList<CrtClasses>();

            foreach (var cClass in context.crtClasses)
            {
                CrtClasses item = new CrtClasses()
                {
                    ID = cClass.classID,
                    ClassName = cClass.className,
                    Description = cClass.description.Clean()
                };

                list.Items.Add(item);
            }

            return new Bag<CrtClasses>(list);
        }

        internal static Bag<CrtCertificates> Certificates()
        {
            IndexedList<CrtCertificates> list = new IndexedList<CrtCertificates>();

            foreach (var certificate in context.crtCertificates)
            {
                CrtCertificates item = new CrtCertificates()
                {
                    ID = certificate.certificateID,
                    Description = certificate.description.Clean()
                };

                if (certificate.categoryID.HasValue)
                    item.CategoryID = certificate.categoryID.Value;

                if (certificate.classID.HasValue)
                    item.ClassID = certificate.classID.Value;

                if (certificate.grade.HasValue)
                    item.Grade = certificate.grade.Value;

                list.Items.Add(item);
            }

            return new Bag<CrtCertificates>(list);
        }

        internal static Bag<CrtRecommendations> CertificateRecommendations()
        {
            IndexedList<CrtRecommendations> list = new IndexedList<CrtRecommendations>();

            foreach (var recommendation in context.crtRecommendations)
            {
                CrtRecommendations item = new CrtRecommendations()
                {
                    ID = recommendation.recommendationID,
                    Level = recommendation.recommendationLevel,
                };

                if (recommendation.certificateID.HasValue)
                    item.CertificateID = recommendation.certificateID.Value;

                if (recommendation.shipTypeID.HasValue)
                    item.ShipTypeID = recommendation.shipTypeID.Value;

                list.Items.Add(item);
            }

            return new Bag<CrtRecommendations>(list);
        }

        internal static Bag<CrtRelationships> CertificateRelationships()
        {
            IndexedList<CrtRelationships> list = new IndexedList<CrtRelationships>();

            foreach (var relationship in context.crtRelationships)
            {
                CrtRelationships item = new CrtRelationships()
                {
                    ID = relationship.relationshipID,
                    ParentID = relationship.parentID,
                    ParentLevel = relationship.parentLevel,
                };

                if (relationship.parentTypeID != 0)
                    item.ParentTypeID = relationship.parentTypeID;

                if (relationship.childID.HasValue)
                    item.ChildID = relationship.childID.Value;

                list.Items.Add(item);
            }

            return new Bag<CrtRelationships>(list);
        }

        internal static RelationSet<DgmTypeAttribute> TypeAttributes()
        {
            List<DgmTypeAttribute> list = new List<DgmTypeAttribute>();

            foreach (var typeAttribute in context.dgmTypeAttributes)
            {
                DgmTypeAttribute item = new DgmTypeAttribute()
                {
                    AttributeID = typeAttribute.attributeID,
                    ItemID = typeAttribute.typeID,
                    ValueFloat = typeAttribute.valueFloat,
                    ValueInt = typeAttribute.valueInt
                };

                list.Add(item);
            }

            return new RelationSet<DgmTypeAttribute>(list);
        }

        internal static RelationSet<InvMetaType> MetaTypes()
        {
            List<InvMetaType> list = new List<InvMetaType>();

            foreach (var metaType in context.invMetaTypes)
            {
                InvMetaType item = new InvMetaType()
                {
                    ItemID = metaType.typeID,
                    MetaGroupID = Convert.ToInt32(metaType.metaGroupID),
                    ParentItemID = Convert.ToInt32(metaType.parentTypeID)
                };

                list.Add(item);
            }

            return new RelationSet<InvMetaType>(list);
        }

        internal static RelationSet<DgmTypeEffect> TypeEffects()
        {
            List<DgmTypeEffect> list = new List<DgmTypeEffect>();

            foreach (var typeEffect in context.dgmTypeEffects)
            {
                DgmTypeEffect item = new DgmTypeEffect()
                {
                    EffectID = typeEffect.effectID,
                    ItemID = typeEffect.typeID
                };

                list.Add(item);
            }

            return new RelationSet<DgmTypeEffect>(list);
        }
    }
}
