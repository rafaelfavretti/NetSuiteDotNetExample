using NetsuiteLibrary.com.netsuite.api.suitetalk.tstdrv2066000;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetsuiteLibrary
{
    public class CustomerExample
    {
        public NetSuiteService service;

        public CustomerExample()
        {
            this.service = ConnectionManager.GetNetSuiteService();
        }

        public void AddCustomer()
        {
            Customer customer = new Customer();
            List<CustomFieldRef> customFields = new List<CustomFieldRef>();

            StringCustomFieldRef customField = new StringCustomFieldRef();
            customField.scriptId = "teste";
            customField.value = "teste";

            customFields.Add(customField);

        }
    }
}
