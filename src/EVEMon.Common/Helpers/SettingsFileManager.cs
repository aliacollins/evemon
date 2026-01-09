using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EVEMon.Common.CloudStorageServices;
using EVEMon.Common.Scheduling;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.SettingsObjects;

namespace EVEMon.Common.Helpers
{
    /// <summary>
    /// Manages the new split settings file structure:
    /// - config.json: UI settings and preferences
    /// - credentials.json: ESI tokens (portable)
    /// - characters/{id}.json: Per-character data
    /// </summary>
    public static class SettingsFileManager
    {
        #region Constants

        private const string ConfigFileName = "config.json";
        private const string CredentialsFileName = "credentials.json";
        private const string CharactersFolderName = "characters";
        private const string CharacterIndexFileName = "index.json";
        private const string LegacySettingsFileName = "settings.xml";

        #endregion

        #region JSON Options

        private static readonly JsonSerializerOptions s_jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };

        private static readonly JsonSerializerOptions s_jsonReadOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        #endregion

        #region Paths

        /// <summary>
        /// Gets the base EVEMon data directory.
        /// </summary>
        public static string DataDirectory => EveMonClient.EVEMonDataDir;

        /// <summary>
        /// Gets the full path to config.json.
        /// </summary>
        public static string ConfigFilePath => Path.Combine(DataDirectory, ConfigFileName);

        /// <summary>
        /// Gets the full path to credentials.json.
        /// </summary>
        public static string CredentialsFilePath => Path.Combine(DataDirectory, CredentialsFileName);

        /// <summary>
        /// Gets the characters folder path.
        /// </summary>
        public static string CharactersDirectory => Path.Combine(DataDirectory, CharactersFolderName);

        /// <summary>
        /// Gets the character index file path.
        /// </summary>
        public static string CharacterIndexFilePath => Path.Combine(CharactersDirectory, CharacterIndexFileName);

        /// <summary>
        /// Gets the legacy settings.xml path.
        /// </summary>
        public static string LegacySettingsFilePath => Path.Combine(DataDirectory, LegacySettingsFileName);

        /// <summary>
        /// Gets the file path for a specific character.
        /// </summary>
        public static string GetCharacterFilePath(long characterId)
            => Path.Combine(CharactersDirectory, $"{characterId}.json");

        #endregion

        #region Directory Management

        /// <summary>
        /// Ensures all required directories exist.
        /// </summary>
        public static void EnsureDirectoriesExist()
        {
            EveMonClient.Trace("begin");

            try
            {
                if (!Directory.Exists(DataDirectory))
                    Directory.CreateDirectory(DataDirectory);

                if (!Directory.Exists(CharactersDirectory))
                    Directory.CreateDirectory(CharactersDirectory);

                EveMonClient.Trace("done");
            }
            catch (Exception ex)
            {
                EveMonClient.Trace($"Error: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Detection

        /// <summary>
        /// Checks if the new JSON settings structure exists.
        /// </summary>
        public static bool JsonSettingsExist()
            => File.Exists(ConfigFilePath);

        /// <summary>
        /// Checks if legacy XML settings exist.
        /// </summary>
        public static bool LegacySettingsExist()
            => File.Exists(LegacySettingsFilePath);

        /// <summary>
        /// Determines if migration from XML to JSON is needed.
        /// </summary>
        public static bool NeedsMigration()
            => LegacySettingsExist() && !JsonSettingsExist();

        /// <summary>
        /// Clears all JSON settings files.
        /// Used when resetting settings to factory defaults.
        /// </summary>
        public static void ClearAllJsonFiles()
        {
            EveMonClient.Trace("begin");

            try
            {
                // Delete config.json
                if (File.Exists(ConfigFilePath))
                    File.Delete(ConfigFilePath);

                // Delete credentials.json
                if (File.Exists(CredentialsFilePath))
                    File.Delete(CredentialsFilePath);

                // Delete entire characters folder
                if (Directory.Exists(CharactersDirectory))
                    Directory.Delete(CharactersDirectory, recursive: true);

                EveMonClient.Trace("All JSON files cleared");
            }
            catch (Exception ex)
            {
                EveMonClient.Trace($"Error clearing JSON files: {ex.Message}");
            }
        }

        /// <summary>
        /// Clears JSON files to force re-migration from XML.
        /// Used when restoring settings from a backup.
        /// </summary>
        public static void ClearForReMigration()
        {
            EveMonClient.Trace("begin");

            try
            {
                // Clear JSON files - they'll be recreated from XML on next startup
                ClearAllJsonFiles();

                // Also restore the migrated settings file if it exists
                string migratedPath = LegacySettingsFilePath + ".migrated";
                if (File.Exists(migratedPath) && !File.Exists(LegacySettingsFilePath))
                {
                    File.Move(migratedPath, LegacySettingsFilePath);
                    EveMonClient.Trace("Restored settings.xml.migrated to settings.xml");
                }

                EveMonClient.Trace("done - JSON cleared for re-migration");
            }
            catch (Exception ex)
            {
                EveMonClient.Trace($"Error clearing for re-migration: {ex.Message}");
            }
        }

        #endregion

        #region Config (UI Settings)

        /// <summary>
        /// Loads the config.json file.
        /// </summary>
        public static async Task<JsonConfig> LoadConfigAsync()
        {
            EveMonClient.Trace("begin");

            if (!File.Exists(ConfigFilePath))
            {
                EveMonClient.Trace("Config file not found, returning defaults");
                return new JsonConfig();
            }

            try
            {
                string json = await File.ReadAllTextAsync(ConfigFilePath);
                var config = JsonSerializer.Deserialize<JsonConfig>(json, s_jsonReadOptions);
                EveMonClient.Trace($"done - loaded {json.Length} bytes");
                return config ?? new JsonConfig();
            }
            catch (Exception ex)
            {
                EveMonClient.Trace($"Error: {ex.Message}");
                return new JsonConfig();
            }
        }

        /// <summary>
        /// Saves the config.json file.
        /// </summary>
        public static async Task SaveConfigAsync(JsonConfig config)
        {
            EveMonClient.Trace("begin");

            try
            {
                EnsureDirectoriesExist();
                string json = JsonSerializer.Serialize(config, s_jsonOptions);
                await WriteFileAtomicAsync(ConfigFilePath, json);
                EveMonClient.Trace($"done - saved {json.Length} bytes");
            }
            catch (Exception ex)
            {
                EveMonClient.Trace($"Error: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Credentials (ESI Tokens)

        /// <summary>
        /// Loads the credentials.json file.
        /// </summary>
        public static async Task<JsonCredentials> LoadCredentialsAsync()
        {
            EveMonClient.Trace("begin");

            if (!File.Exists(CredentialsFilePath))
            {
                EveMonClient.Trace("Credentials file not found, returning empty");
                return new JsonCredentials();
            }

            try
            {
                string json = await File.ReadAllTextAsync(CredentialsFilePath);
                var creds = JsonSerializer.Deserialize<JsonCredentials>(json, s_jsonReadOptions);
                EveMonClient.Trace($"done - loaded {creds?.EsiKeys?.Count ?? 0} ESI keys");
                return creds ?? new JsonCredentials();
            }
            catch (Exception ex)
            {
                EveMonClient.Trace($"Error: {ex.Message}");
                return new JsonCredentials();
            }
        }

        /// <summary>
        /// Saves the credentials.json file.
        /// </summary>
        public static async Task SaveCredentialsAsync(JsonCredentials credentials)
        {
            EveMonClient.Trace("begin");

            try
            {
                EnsureDirectoriesExist();
                string json = JsonSerializer.Serialize(credentials, s_jsonOptions);
                await WriteFileAtomicAsync(CredentialsFilePath, json);
                EveMonClient.Trace($"done - saved {credentials?.EsiKeys?.Count ?? 0} ESI keys");
            }
            catch (Exception ex)
            {
                EveMonClient.Trace($"Error: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Character Index

        /// <summary>
        /// Loads the character index (list of all characters).
        /// </summary>
        public static async Task<JsonCharacterIndex> LoadCharacterIndexAsync()
        {
            EveMonClient.Trace("begin");

            if (!File.Exists(CharacterIndexFilePath))
            {
                EveMonClient.Trace("Character index not found, returning empty");
                return new JsonCharacterIndex();
            }

            try
            {
                string json = await File.ReadAllTextAsync(CharacterIndexFilePath);
                var index = JsonSerializer.Deserialize<JsonCharacterIndex>(json, s_jsonReadOptions);
                EveMonClient.Trace($"done - loaded {index?.Characters?.Count ?? 0} character entries");
                return index ?? new JsonCharacterIndex();
            }
            catch (Exception ex)
            {
                EveMonClient.Trace($"Error: {ex.Message}");
                return new JsonCharacterIndex();
            }
        }

        /// <summary>
        /// Saves the character index.
        /// </summary>
        public static async Task SaveCharacterIndexAsync(JsonCharacterIndex index)
        {
            EveMonClient.Trace("begin");

            try
            {
                EnsureDirectoriesExist();
                string json = JsonSerializer.Serialize(index, s_jsonOptions);
                await WriteFileAtomicAsync(CharacterIndexFilePath, json);
                EveMonClient.Trace($"done - saved {index?.Characters?.Count ?? 0} character entries");
            }
            catch (Exception ex)
            {
                EveMonClient.Trace($"Error: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Character Data

        /// <summary>
        /// Loads a specific character's data.
        /// </summary>
        public static async Task<JsonCharacterData> LoadCharacterAsync(long characterId)
        {
            EveMonClient.Trace($"begin - character {characterId}");

            string filePath = GetCharacterFilePath(characterId);
            if (!File.Exists(filePath))
            {
                EveMonClient.Trace($"Character file not found: {characterId}");
                return null;
            }

            try
            {
                string json = await File.ReadAllTextAsync(filePath);
                var character = JsonSerializer.Deserialize<JsonCharacterData>(json, s_jsonReadOptions);
                EveMonClient.Trace($"done - loaded character {characterId} ({json.Length} bytes)");
                return character;
            }
            catch (Exception ex)
            {
                EveMonClient.Trace($"Error loading character {characterId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Saves a specific character's data.
        /// </summary>
        public static async Task SaveCharacterAsync(JsonCharacterData character)
        {
            if (character == null)
                return;

            EveMonClient.Trace($"begin - character {character.CharacterId}");

            try
            {
                EnsureDirectoriesExist();
                string filePath = GetCharacterFilePath(character.CharacterId);
                string json = JsonSerializer.Serialize(character, s_jsonOptions);
                await WriteFileAtomicAsync(filePath, json);
                EveMonClient.Trace($"done - saved character {character.CharacterId} ({json.Length} bytes)");
            }
            catch (Exception ex)
            {
                EveMonClient.Trace($"Error saving character {character.CharacterId}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deletes a character's data file.
        /// </summary>
        public static void DeleteCharacter(long characterId)
        {
            EveMonClient.Trace($"begin - character {characterId}");

            string filePath = GetCharacterFilePath(characterId);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                EveMonClient.Trace($"done - deleted character {characterId}");
            }
        }

        /// <summary>
        /// Gets all character IDs that have data files.
        /// </summary>
        public static IEnumerable<long> GetSavedCharacterIds()
        {
            if (!Directory.Exists(CharactersDirectory))
                yield break;

            foreach (string file in Directory.GetFiles(CharactersDirectory, "*.json"))
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (fileName == "index")
                    continue;

                if (long.TryParse(fileName, out long characterId))
                    yield return characterId;
            }
        }

        #endregion

        #region Atomic File Writing

        /// <summary>
        /// Writes a file atomically using a temp file and rename.
        /// </summary>
        private static async Task WriteFileAtomicAsync(string filePath, string content)
        {
            string directory = Path.GetDirectoryName(filePath);
            string tempPath = Path.Combine(directory, $".{Path.GetFileName(filePath)}.tmp");

            try
            {
                // Write to temp file
                await File.WriteAllTextAsync(tempPath, content);

                // Delete target if exists
                if (File.Exists(filePath))
                    File.Delete(filePath);

                // Rename temp to target
                File.Move(tempPath, filePath);
            }
            finally
            {
                // Clean up temp file if it still exists
                if (File.Exists(tempPath))
                {
                    try { File.Delete(tempPath); }
                    catch { /* Ignore cleanup errors */ }
                }
            }
        }

        #endregion

        #region Migration from XML

        /// <summary>
        /// Migrates settings from the legacy XML format to the new JSON format.
        /// </summary>
        /// <param name="xmlSettings">The deserialized XML settings.</param>
        public static async Task MigrateFromXmlAsync(SerializableSettings xmlSettings)
        {
            EveMonClient.Trace("begin - migrating from XML to JSON");

            if (xmlSettings == null)
            {
                EveMonClient.Trace("No XML settings to migrate");
                return;
            }

            try
            {
                EnsureDirectoriesExist();

                // 1. Migrate config (UI settings, preferences)
                var config = MigrateConfig(xmlSettings);
                await SaveConfigAsync(config);
                EveMonClient.Trace("Config migrated");

                // 2. Migrate credentials (ESI keys)
                var credentials = MigrateCredentials(xmlSettings);
                await SaveCredentialsAsync(credentials);
                EveMonClient.Trace($"Credentials migrated - {credentials.EsiKeys.Count} ESI keys");

                // 3. Migrate characters to individual files
                var index = await MigrateCharactersAsync(xmlSettings);
                await SaveCharacterIndexAsync(index);
                EveMonClient.Trace($"Characters migrated - {index.Characters.Count} characters");

                // 4. Rename old settings.xml to settings.xml.migrated
                string migratedPath = LegacySettingsFilePath + ".migrated";
                if (File.Exists(LegacySettingsFilePath))
                {
                    if (File.Exists(migratedPath))
                        File.Delete(migratedPath);
                    File.Move(LegacySettingsFilePath, migratedPath);
                    EveMonClient.Trace("Renamed settings.xml to settings.xml.migrated");
                }

                EveMonClient.Trace("done - migration complete");
            }
            catch (Exception ex)
            {
                EveMonClient.Trace($"Error during migration: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Migrates config settings from XML.
        /// </summary>
        private static JsonConfig MigrateConfig(SerializableSettings xml)
        {
            return new JsonConfig
            {
                Version = 1,
                ForkId = xml.ForkId ?? "aliacollins",
                ForkVersion = xml.ForkVersion,
                LastSaved = DateTime.UtcNow,
                UI = xml.UI,
                G15 = xml.G15,
                Proxy = xml.Proxy,
                Updates = xml.Updates,
                Calendar = xml.Calendar,
                Exportation = xml.Exportation,
                MarketPricer = xml.MarketPricer,
                Notifications = xml.Notifications,
                LoadoutsProvider = xml.LoadoutsProvider,
                PortableEveInstallations = xml.PortableEveInstallations,
                CloudStorageServiceProvider = xml.CloudStorageServiceProvider,
                Scheduler = xml.Scheduler
            };
        }

        /// <summary>
        /// Migrates ESI credentials from XML.
        /// </summary>
        private static JsonCredentials MigrateCredentials(SerializableSettings xml)
        {
            var credentials = new JsonCredentials
            {
                Version = 1,
                LastSaved = DateTime.UtcNow
            };

            foreach (var esiKey in xml.ESIKeys)
            {
                credentials.EsiKeys.Add(new JsonEsiKey
                {
                    CharacterId = esiKey.ID,
                    RefreshToken = esiKey.RefreshToken,
                    AccessMask = esiKey.AccessMask,
                    Monitored = esiKey.Monitored
                });
            }

            return credentials;
        }

        /// <summary>
        /// Migrates characters from XML to individual JSON files.
        /// </summary>
        private static async Task<JsonCharacterIndex> MigrateCharactersAsync(SerializableSettings xml)
        {
            var index = new JsonCharacterIndex
            {
                Version = 1,
                LastSaved = DateTime.UtcNow
            };

            // Build a map of character Guid to character ID
            var guidToId = new Dictionary<Guid, long>();
            foreach (var character in xml.Characters)
            {
                guidToId[character.Guid] = character.ID;
            }

            // Track monitored character IDs
            foreach (var monitored in xml.MonitoredCharacters)
            {
                if (guidToId.TryGetValue(monitored.CharacterGuid, out long charId))
                {
                    index.MonitoredCharacterIds.Add(charId);
                }
            }

            // Get plans grouped by character Guid (plans use Guid as owner)
            var plansByGuid = xml.Plans
                .GroupBy(p => p.Owner)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Migrate each character
            foreach (var character in xml.Characters)
            {
                // Get plans for this character by matching Guid
                plansByGuid.TryGetValue(character.Guid, out var characterPlans);

                var characterData = MigrateCharacter(character, characterPlans);
                if (characterData != null)
                {
                    await SaveCharacterAsync(characterData);

                    index.Characters.Add(new JsonCharacterIndexEntry
                    {
                        CharacterId = characterData.CharacterId,
                        Name = characterData.Name,
                        CorporationName = characterData.CorporationName,
                        AllianceName = characterData.AllianceName,
                        IsUriCharacter = character is SerializableUriCharacter,
                        LastUpdated = DateTime.UtcNow
                    });
                }
            }

            return index;
        }

        /// <summary>
        /// Migrates a single character from XML to JSON format.
        /// </summary>
        private static JsonCharacterData MigrateCharacter(
            SerializableSettingsCharacter xml,
            List<SerializablePlan> characterPlans)
        {
            if (xml == null)
                return null;

            var data = new JsonCharacterData
            {
                Version = 1,
                CharacterId = xml.ID,
                LastSaved = DateTime.UtcNow,
                Name = xml.Name,
                Birthday = xml.Birthday,
                Race = xml.Race,
                Bloodline = xml.BloodLine,
                Ancestry = xml.Ancestry,
                Gender = xml.Gender,
                CorporationId = xml.CorporationID,
                CorporationName = xml.CorporationName,
                AllianceId = xml.AllianceID,
                AllianceName = xml.AllianceName,
                FactionId = xml.FactionID,
                FactionName = xml.FactionName,
                Intelligence = (int)(xml.Attributes?.Intelligence ?? 0),
                Memory = (int)(xml.Attributes?.Memory ?? 0),
                Charisma = (int)(xml.Attributes?.Charisma ?? 0),
                Perception = (int)(xml.Attributes?.Perception ?? 0),
                Willpower = (int)(xml.Attributes?.Willpower ?? 0),
                Balance = xml.Balance,
                FreeSkillPoints = xml.FreeSkillPoints
            };

            // Migrate skills
            if (xml.Skills != null)
            {
                foreach (var skill in xml.Skills)
                {
                    data.Skills.Add(new JsonSkill
                    {
                        TypeId = skill.ID,
                        Level = (int)skill.Level,
                        Skillpoints = skill.Skillpoints,
                        IsKnown = skill.IsKnown,
                        OwnsBook = skill.OwnsBook
                    });
                }
            }

            // Migrate skill queue (only available on CCP characters)
            if (xml is SerializableCCPCharacter ccpCharacter && ccpCharacter.SkillQueue != null)
            {
                foreach (var queueItem in ccpCharacter.SkillQueue)
                {
                    data.SkillQueue.Add(new JsonSkillQueueEntry
                    {
                        TypeId = queueItem.ID,
                        Level = queueItem.Level,
                        StartTime = queueItem.StartTime,
                        EndTime = queueItem.EndTime,
                        StartSP = queueItem.StartSP,
                        EndSP = queueItem.EndSP
                    });
                }
            }

            // Migrate implant sets
            if (xml.ImplantSets != null)
            {
                // Active clone
                if (xml.ImplantSets.ActiveClone != null)
                {
                    data.ImplantSets.Add(MigrateImplantSet(xml.ImplantSets.ActiveClone, "Active Clone"));
                }

                // Jump clones
                if (xml.ImplantSets.JumpClones != null)
                {
                    int cloneNum = 1;
                    foreach (var jumpClone in xml.ImplantSets.JumpClones)
                    {
                        data.ImplantSets.Add(MigrateImplantSet(jumpClone, $"Jump Clone {cloneNum++}"));
                    }
                }

                // Custom sets
                foreach (var customSet in xml.ImplantSets.CustomSets)
                {
                    data.ImplantSets.Add(MigrateImplantSet(customSet, customSet.Name));
                }
            }

            // Migrate plans for this character
            if (characterPlans != null)
            {
                foreach (var plan in characterPlans)
                {
                    var jsonPlan = new JsonPlan
                    {
                        Name = plan.Name,
                        Description = plan.Description
                    };

                    if (plan.Entries != null)
                    {
                        foreach (var entry in plan.Entries)
                        {
                            jsonPlan.Entries.Add(new JsonPlanEntry
                            {
                                SkillId = entry.ID,
                                Level = (int)entry.Level,
                                Type = entry.Type.ToString(),
                                Priority = entry.Priority,
                                Notes = entry.Notes
                            });
                        }
                    }
                    data.Plans.Add(jsonPlan);
                }
            }

            return data;
        }

        /// <summary>
        /// Migrates an implant set from XML format.
        /// </summary>
        private static JsonImplantSet MigrateImplantSet(SerializableSettingsImplantSet xmlSet, string name)
        {
            var jsonSet = new JsonImplantSet
            {
                Name = string.IsNullOrEmpty(xmlSet.Name) ? name : xmlSet.Name
            };

            // The implant set stores implant names/IDs as strings per slot
            // We'll convert these to our simpler format
            AddImplantIfValid(jsonSet, 1, xmlSet.Intelligence);
            AddImplantIfValid(jsonSet, 2, xmlSet.Memory);
            AddImplantIfValid(jsonSet, 3, xmlSet.Willpower);
            AddImplantIfValid(jsonSet, 4, xmlSet.Perception);
            AddImplantIfValid(jsonSet, 5, xmlSet.Charisma);
            AddImplantIfValid(jsonSet, 6, xmlSet.Slot6);
            AddImplantIfValid(jsonSet, 7, xmlSet.Slot7);
            AddImplantIfValid(jsonSet, 8, xmlSet.Slot8);
            AddImplantIfValid(jsonSet, 9, xmlSet.Slot9);
            AddImplantIfValid(jsonSet, 10, xmlSet.Slot10);

            return jsonSet;
        }

        /// <summary>
        /// Adds an implant to the set if the value is not "None" or empty.
        /// </summary>
        private static void AddImplantIfValid(JsonImplantSet set, int slot, string implantValue)
        {
            if (string.IsNullOrEmpty(implantValue) || implantValue == "None")
                return;

            // The value might be an implant name or ID - store whatever we have
            set.Implants.Add(new JsonImplant
            {
                Slot = slot,
                TypeId = 0,  // We'll store the name in a separate property if needed
                Name = implantValue
            });
        }

        #endregion

        #region Save from SerializableSettings

        /// <summary>
        /// Saves settings to JSON format from SerializableSettings.
        /// Called alongside XML save to keep JSON files in sync.
        /// </summary>
        /// <param name="settings">The serializable settings to save.</param>
        public static async Task SaveFromSerializableSettingsAsync(SerializableSettings settings)
        {
            if (settings == null)
            {
                EveMonClient.Trace("SaveFromSerializableSettingsAsync: No settings to save");
                return;
            }

            // Only save to JSON if migration has already occurred
            if (!JsonSettingsExist())
            {
                // JSON files don't exist yet - skip JSON save (migration will happen on next startup)
                return;
            }

            try
            {
                EnsureDirectoriesExist();

                // Save config
                var config = MigrateConfig(settings);
                await SaveConfigAsync(config);

                // Save credentials
                var credentials = MigrateCredentials(settings);
                await SaveCredentialsAsync(credentials);

                // Save characters
                await SaveCharactersFromXmlAsync(settings);

                EveMonClient.Trace($"SaveFromSerializableSettingsAsync: Saved {settings.Characters.Count} characters to JSON");
            }
            catch (Exception ex)
            {
                // Log but don't fail - XML is primary, JSON is backup
                EveMonClient.Trace($"SaveFromSerializableSettingsAsync: Error saving JSON (non-critical): {ex.Message}");
            }
        }

        /// <summary>
        /// Saves characters from SerializableSettings to JSON files.
        /// </summary>
        private static async Task SaveCharactersFromXmlAsync(SerializableSettings xml)
        {
            var index = new JsonCharacterIndex
            {
                Version = 1,
                LastSaved = DateTime.UtcNow
            };

            // Build a map of character Guid to character ID
            var guidToId = new Dictionary<Guid, long>();
            foreach (var character in xml.Characters)
            {
                guidToId[character.Guid] = character.ID;
            }

            // Track monitored character IDs
            foreach (var monitored in xml.MonitoredCharacters)
            {
                if (guidToId.TryGetValue(monitored.CharacterGuid, out long charId))
                {
                    index.MonitoredCharacterIds.Add(charId);
                }
            }

            // Get plans grouped by character Guid
            var plansByGuid = xml.Plans
                .GroupBy(p => p.Owner)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Save each character
            foreach (var character in xml.Characters)
            {
                plansByGuid.TryGetValue(character.Guid, out var characterPlans);

                var characterData = MigrateCharacter(character, characterPlans);
                if (characterData != null)
                {
                    await SaveCharacterAsync(characterData);

                    index.Characters.Add(new JsonCharacterIndexEntry
                    {
                        CharacterId = characterData.CharacterId,
                        Name = characterData.Name,
                        CorporationName = characterData.CorporationName,
                        AllianceName = characterData.AllianceName,
                        IsUriCharacter = character is SerializableUriCharacter,
                        LastUpdated = DateTime.UtcNow
                    });
                }
            }

            // Remove orphaned character files
            CleanupOrphanedCharacterFiles(xml.Characters.Select(c => c.ID).ToHashSet());

            await SaveCharacterIndexAsync(index);
        }

        /// <summary>
        /// Removes character JSON files that no longer exist in settings.
        /// </summary>
        private static void CleanupOrphanedCharacterFiles(HashSet<long> validCharacterIds)
        {
            try
            {
                if (!Directory.Exists(CharactersDirectory))
                    return;

                foreach (var file in Directory.GetFiles(CharactersDirectory, "*.json"))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (fileName == "index")
                        continue;

                    if (long.TryParse(fileName, out long charId) && !validCharacterIds.Contains(charId))
                    {
                        try
                        {
                            File.Delete(file);
                            EveMonClient.Trace($"Cleaned up orphaned character file: {fileName}.json");
                        }
                        catch
                        {
                            // Ignore deletion errors
                        }
                    }
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        #endregion

        #region Combined Backup Format

        /// <summary>
        /// Exports all settings to a single combined JSON backup file.
        /// Used by File > Save Settings menu.
        /// </summary>
        /// <param name="filePath">The path to save the backup to.</param>
        /// <param name="settings">The settings to export.</param>
        public static async Task ExportBackupAsync(string filePath, SerializableSettings settings)
        {
            EveMonClient.Trace($"begin - exporting to {filePath}");

            if (settings == null)
            {
                EveMonClient.Trace("No settings to export");
                return;
            }

            try
            {
                var backup = new JsonBackup
                {
                    Version = 1,
                    ForkId = settings.ForkId ?? "aliacollins",
                    ForkVersion = settings.ForkVersion,
                    ExportedAt = DateTime.UtcNow,
                    Config = MigrateConfig(settings),
                    Credentials = MigrateCredentials(settings),
                    Characters = new List<JsonCharacterData>()
                };

                // Build a map of character Guid to character ID
                var guidToId = new Dictionary<Guid, long>();
                foreach (var character in settings.Characters)
                {
                    guidToId[character.Guid] = character.ID;
                }

                // Get plans grouped by character Guid
                var plansByGuid = settings.Plans
                    .GroupBy(p => p.Owner)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Track monitored character IDs
                backup.MonitoredCharacterIds = new List<long>();
                foreach (var monitored in settings.MonitoredCharacters)
                {
                    if (guidToId.TryGetValue(monitored.CharacterGuid, out long charId))
                    {
                        backup.MonitoredCharacterIds.Add(charId);
                    }
                }

                // Export each character
                foreach (var character in settings.Characters)
                {
                    plansByGuid.TryGetValue(character.Guid, out var characterPlans);
                    var characterData = MigrateCharacter(character, characterPlans);
                    if (characterData != null)
                    {
                        backup.Characters.Add(characterData);
                    }
                }

                // Serialize and write
                string json = JsonSerializer.Serialize(backup, s_jsonOptions);
                await WriteFileAtomicAsync(filePath, json);

                EveMonClient.Trace($"done - exported {backup.Characters.Count} characters to backup");
            }
            catch (Exception ex)
            {
                EveMonClient.Trace($"Error exporting backup: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Imports settings from a combined JSON backup file.
        /// Used by File > Restore Settings menu.
        /// </summary>
        /// <param name="filePath">The path to the backup file.</param>
        /// <returns>True if import was successful.</returns>
        public static async Task<bool> ImportBackupAsync(string filePath)
        {
            EveMonClient.Trace($"begin - importing from {filePath}");

            try
            {
                string json = await File.ReadAllTextAsync(filePath);
                var backup = JsonSerializer.Deserialize<JsonBackup>(json, s_jsonOptions);

                if (backup == null)
                {
                    EveMonClient.Trace("Failed to deserialize backup");
                    return false;
                }

                // Clear existing JSON files
                ClearAllJsonFiles();
                EnsureDirectoriesExist();

                // Save config
                if (backup.Config != null)
                {
                    await SaveConfigAsync(backup.Config);
                }

                // Save credentials
                if (backup.Credentials != null)
                {
                    await SaveCredentialsAsync(backup.Credentials);
                }

                // Save characters
                var index = new JsonCharacterIndex
                {
                    Version = 1,
                    LastSaved = DateTime.UtcNow,
                    MonitoredCharacterIds = backup.MonitoredCharacterIds ?? new List<long>()
                };

                foreach (var character in backup.Characters ?? new List<JsonCharacterData>())
                {
                    await SaveCharacterAsync(character);
                    index.Characters.Add(new JsonCharacterIndexEntry
                    {
                        CharacterId = character.CharacterId,
                        Name = character.Name,
                        CorporationName = character.CorporationName,
                        AllianceName = character.AllianceName,
                        LastUpdated = DateTime.UtcNow
                    });
                }

                await SaveCharacterIndexAsync(index);

                EveMonClient.Trace($"done - imported {backup.Characters?.Count ?? 0} characters from backup");
                return true;
            }
            catch (Exception ex)
            {
                EveMonClient.Trace($"Error importing backup: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if a file is a JSON backup file.
        /// </summary>
        public static bool IsJsonBackupFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            if (extension != ".json")
                return false;

            try
            {
                // Quick check - look for our backup format markers
                string content = File.ReadAllText(filePath);
                return content.Contains("\"ForkId\"") && content.Contains("\"Characters\"");
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }

    #region JSON Data Classes

    /// <summary>
    /// Combined backup format - all settings in one file for export/import.
    /// </summary>
    public class JsonBackup
    {
        public int Version { get; set; } = 1;
        public string ForkId { get; set; }
        public string ForkVersion { get; set; }
        public DateTime ExportedAt { get; set; }
        public JsonConfig Config { get; set; }
        public JsonCredentials Credentials { get; set; }
        public List<JsonCharacterData> Characters { get; set; } = new List<JsonCharacterData>();
        public List<long> MonitoredCharacterIds { get; set; } = new List<long>();
    }

    /// <summary>
    /// Root config.json structure - UI settings and preferences.
    /// </summary>
    public class JsonConfig
    {
        public int Version { get; set; } = 1;
        public string ForkId { get; set; } = "aliacollins";
        public string ForkVersion { get; set; }
        public DateTime LastSaved { get; set; } = DateTime.UtcNow;

        // Settings objects (will be populated from existing settings classes)
        public UISettings UI { get; set; }
        public G15Settings G15 { get; set; }
        public ProxySettings Proxy { get; set; }
        public UpdateSettings Updates { get; set; }
        public CalendarSettings Calendar { get; set; }
        public ExportationSettings Exportation { get; set; }
        public MarketPricerSettings MarketPricer { get; set; }
        public NotificationSettings Notifications { get; set; }
        public LoadoutsProviderSettings LoadoutsProvider { get; set; }
        public PortableEveInstallationsSettings PortableEveInstallations { get; set; }
        public CloudStorageServiceProviderSettings CloudStorageServiceProvider { get; set; }
        public SchedulerSettings Scheduler { get; set; }
    }

    /// <summary>
    /// Root credentials.json structure - ESI authentication tokens.
    /// </summary>
    public class JsonCredentials
    {
        public int Version { get; set; } = 1;
        public DateTime LastSaved { get; set; } = DateTime.UtcNow;
        public List<JsonEsiKey> EsiKeys { get; set; } = new List<JsonEsiKey>();
    }

    /// <summary>
    /// ESI key data for credentials.json.
    /// </summary>
    public class JsonEsiKey
    {
        public long CharacterId { get; set; }
        public string RefreshToken { get; set; }
        public ulong AccessMask { get; set; }
        public bool Monitored { get; set; }
    }

    /// <summary>
    /// Character index structure - lightweight list of all characters.
    /// </summary>
    public class JsonCharacterIndex
    {
        public int Version { get; set; } = 1;
        public DateTime LastSaved { get; set; } = DateTime.UtcNow;
        public List<JsonCharacterIndexEntry> Characters { get; set; } = new List<JsonCharacterIndexEntry>();
        public List<long> MonitoredCharacterIds { get; set; } = new List<long>();
    }

    /// <summary>
    /// Lightweight character entry for the index.
    /// </summary>
    public class JsonCharacterIndexEntry
    {
        public long CharacterId { get; set; }
        public string Name { get; set; }
        public string CorporationName { get; set; }
        public string AllianceName { get; set; }
        public bool IsUriCharacter { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// Full character data structure for {characterId}.json.
    /// </summary>
    public class JsonCharacterData
    {
        public int Version { get; set; } = 1;
        public long CharacterId { get; set; }
        public DateTime LastSaved { get; set; } = DateTime.UtcNow;

        // Character identity
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Race { get; set; }
        public string Bloodline { get; set; }
        public string Ancestry { get; set; }
        public string Gender { get; set; }

        // Corporation/Alliance
        public long CorporationId { get; set; }
        public string CorporationName { get; set; }
        public long AllianceId { get; set; }
        public string AllianceName { get; set; }
        public long FactionId { get; set; }
        public string FactionName { get; set; }

        // Attributes
        public int Intelligence { get; set; }
        public int Memory { get; set; }
        public int Charisma { get; set; }
        public int Perception { get; set; }
        public int Willpower { get; set; }

        // Financial
        public decimal Balance { get; set; }

        // Skills and training
        public List<JsonSkill> Skills { get; set; } = new List<JsonSkill>();
        public List<JsonSkillQueueEntry> SkillQueue { get; set; } = new List<JsonSkillQueueEntry>();
        public int FreeSkillPoints { get; set; }

        // Implants
        public List<JsonImplantSet> ImplantSets { get; set; } = new List<JsonImplantSet>();

        // Plans
        public List<JsonPlan> Plans { get; set; } = new List<JsonPlan>();

        // Cached API data
        public List<JsonMarketOrder> MarketOrders { get; set; } = new List<JsonMarketOrder>();
        public List<JsonContract> Contracts { get; set; } = new List<JsonContract>();
        public List<JsonIndustryJob> IndustryJobs { get; set; } = new List<JsonIndustryJob>();
        public List<JsonAsset> Assets { get; set; } = new List<JsonAsset>();
        public List<JsonWalletJournalEntry> WalletJournal { get; set; } = new List<JsonWalletJournalEntry>();
        public List<JsonWalletTransaction> WalletTransactions { get; set; } = new List<JsonWalletTransaction>();

        // Last update times for API data
        public Dictionary<string, DateTime> LastApiUpdates { get; set; } = new Dictionary<string, DateTime>();
    }

    // Placeholder classes for nested data - will be implemented fully later
    public class JsonSkill
    {
        public int TypeId { get; set; }
        public int Level { get; set; }
        public long Skillpoints { get; set; }
        public bool IsKnown { get; set; }
        public bool OwnsBook { get; set; }
    }

    public class JsonSkillQueueEntry
    {
        public int TypeId { get; set; }
        public int Level { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int StartSP { get; set; }
        public int EndSP { get; set; }
    }

    public class JsonImplantSet
    {
        public string Name { get; set; }
        public List<JsonImplant> Implants { get; set; } = new List<JsonImplant>();
    }

    public class JsonImplant
    {
        public int Slot { get; set; }
        public int TypeId { get; set; }
        public int Bonus { get; set; }
        public string Name { get; set; }
    }

    public class JsonPlan
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<JsonPlanEntry> Entries { get; set; } = new List<JsonPlanEntry>();
    }

    public class JsonPlanEntry
    {
        public int SkillId { get; set; }
        public int Level { get; set; }
        public string Type { get; set; }
        public int Priority { get; set; }
        public string Notes { get; set; }
    }

    public class JsonMarketOrder { /* Will be fully implemented */ }
    public class JsonContract { /* Will be fully implemented */ }
    public class JsonIndustryJob { /* Will be fully implemented */ }
    public class JsonAsset { /* Will be fully implemented */ }
    public class JsonWalletJournalEntry { /* Will be fully implemented */ }
    public class JsonWalletTransaction { /* Will be fully implemented */ }

    #endregion
}
