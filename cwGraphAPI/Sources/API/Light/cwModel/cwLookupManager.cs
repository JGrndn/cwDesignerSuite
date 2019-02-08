using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Casewise.Data.ICM;
using System.Data;

namespace Casewise.GraphAPI.API
{
    /// <summary>
    /// Manage the Lookups
    /// </summary>
    public class cwLookupManager
    {
        private Dictionary<int, string[]> lookups = new Dictionary<int, string[]>();
        private Dictionary<int, Dictionary<int, cwLookup>> lookupsByPropertyTypeIDAndLookupID = new Dictionary<int, Dictionary<int, cwLookup>>();
        
        private const int ABBR_POSITION_IN_LOOKUPS = 0;
        private const int NAME_POSITION_IN_LOOKUPS = 1;
        private cwLightModel currentModel = null;

        /// <summary>
        /// Allow to select a lookup ID
        /// </summary>
        public const string LOOKUPID_KEY = "_LOOKUPID";

        /// <summary>
        /// Allow to select a lookup abbreviation
        /// </summary>
        public const string LOOKUPABBR_KEY = "_LOOKUPABBR";

        /// <summary>
        /// Initializes a new instance of the <see cref="cwLookupManager"/> class.
        /// </summary>
        /// <param name="currentModel">The current model.</param>
        public cwLookupManager(cwLightModel currentModel)
        {
            this.currentModel = currentModel;
            loadLookups();
        }

        /// <summary>
        /// Creates the lookup value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="lookupName">Name of the lookup.</param>
        public void createLookupValue(cwLightProperty property, string lookupName)
        {
            cwLookup lookup = property.lookupContent.Find(l => l.Equals(lookupName));
            if (lookup != null)
            {
                // value already exists in the lookup
                return;
            }
            else
            {
                if (!"Lookup".Equals(property.DataType)) return;
                StringBuilder query = new StringBuilder();
                query.Append("INSERT INTO LOOKUP (ID, NAME, PROPERTYTYPEID) VALUES (@@IDENTITY, @NAME, @PROPERTYID)");
                ICMCommand command = new ICMCommand(query.ToString(), currentModel.getConnection());
                command.Parameters.Add("@NAME", lookupName);
                command.Parameters.Add("@PROPERTYID", property.ID.ToString());
                using (IDbTransaction trans = currentModel.getConnection().BeginTransaction())
                {
                    IDataReader reader = command.ExecuteReader();
                    trans.Commit();
                }

                //reload lookup manager
                currentModel.lookupManager = new cwLookupManager(currentModel);
            }
        }

        /// <summary>
        /// Gets the lookups for property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        internal List<cwLookup> getLookupsForProperty(cwLightProperty property)
        {
            if (!lookupsByPropertyTypeIDAndLookupID.ContainsKey(property.ID))
            {
                return new List<cwLookup>();
            }
            List<cwLookup> lookups = lookupsByPropertyTypeIDAndLookupID[property.ID].Values.ToList<cwLookup>();
            lookups.Sort(delegate(cwLookup l1, cwLookup l2) { return l1.Name.CompareTo(l2.Name); });
            return lookups;
        }

        /// <summary>
        /// Loads the lookups.
        /// </summary>
        private void loadLookups()
        {
            try
            {
                lookups = new Dictionary<int, string[]>();
                lookupsByPropertyTypeIDAndLookupID = new Dictionary<int, Dictionary<int, cwLookup>>();
                StringBuilder query = new StringBuilder();
                query.Append("SELECT ID, ABBREVIATION, NAME, UNIQUEIDENTIFIER, PROPERTYTYPEID FROM LOOKUP");

                using (ICMCommand command = new ICMCommand(query.ToString(), currentModel.getConnection()))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string abbr = reader.GetString(1);
                            string name = reader.GetString(2);
                            string uuid = reader.GetString(3);
                            int propertyTypeID = reader.GetInt32(4);
                            lookups[id] = new string[] { abbr, name };
                            if (!lookupsByPropertyTypeIDAndLookupID.ContainsKey(propertyTypeID))
                            {
                                lookupsByPropertyTypeIDAndLookupID[propertyTypeID] = new Dictionary<int, cwLookup>();
                            }
                            cwLookup lookup = new cwLookup(id, abbr, name, uuid);
                            lookupsByPropertyTypeIDAndLookupID[propertyTypeID][id] = lookup;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Gets the lookups count.
        /// </summary>
        /// <returns></returns>
        public int getLookupsCount()
        {
            return lookups.Count;
        }

        /// <summary>
        /// Determines whether [has lookup ID] [the specified lookup ID].
        /// </summary>
        /// <param name="lookupID">The lookup ID.</param>
        /// <returns>
        ///   <c>true</c> if [has lookup ID] [the specified lookup ID]; otherwise, <c>false</c>.
        /// </returns>
        public bool hasLookupID(int lookupID)
        {
            return lookups.ContainsKey(lookupID);
        }

        /// <summary>
        /// Gets the lookup name by ID.
        /// </summary>
        /// <param name="lookupID">The lookup ID.</param>
        /// <returns></returns>
        public string getLookupNameByID(int lookupID)
        {
            if (hasLookupID(lookupID))
            {
                return lookups[lookupID][NAME_POSITION_IN_LOOKUPS];
            }
            return null;
        }

        /// <summary>
        /// Gets the lookup abbreviation by ID.
        /// </summary>
        /// <param name="lookupID">The lookup ID.</param>
        /// <returns></returns>
        public string getLookupAbbreviationByID(int lookupID)
        {
            if (hasLookupID(lookupID))
            {
                return lookups[lookupID][ABBR_POSITION_IN_LOOKUPS];
            }
            return null;
        }

    }
}
