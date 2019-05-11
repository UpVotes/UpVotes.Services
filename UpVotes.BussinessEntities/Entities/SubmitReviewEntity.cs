using System;
using System.Collections.Generic;

namespace UpVotes.BusinessEntities.Entities
{
    public class AutocompleteRequestEntity
    {
        public int Type { get; set; }
        public string Search { get; set; }
    }
    public class AutocompleteResponseEntity
    {
        public List<Autocomplete> companySoftwareAutocomplete { get; set; }
        
    }
    public class Autocomplete
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
