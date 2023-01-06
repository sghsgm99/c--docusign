using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using static DocuSign.eSign.Api.EnvelopesApi;
using static DocuSign.eSign.Client.Auth.OAuth;
using static DocuSign.eSign.Client.Auth.OAuth.UserInfo;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace ConsoleApp2
{
    internal class Program
    {
        private static string accountBaseUri = @"account-d.docusign.com";
        private static string baseUrl = @"demo.docusign.net/restapi";
        private static string docuSignLoginUrl = @"account-d.docusign.com";
        private static string authToken = "eyJ0eXAiOiJNVCIsImFsZyI6IlJTMjU2Iiwia2lkIjoiNjgxODVmZjEtNGU1MS00Y2U5LWFmMWMtNjg5ODEyMjAzMzE3In0.AQoAAAABAAUABwAAnXWnstfaSAgAAAU6CbvX2kgCABzoxDebUz5LpoL1naYeHrAVAAEAAAAYAAIAAAAFAAAAHQAAAA0AJAAAADg5YmE5MDMwLWRkMGItNDI0Yi1iOTI4LThhOTYxMTAxZmU4ZiIAJAAAADg5YmE5MDMwLWRkMGItNDI0Yi1iOTI4LThhOTYxMTAxZmU4ZhIAAQAAAAYAAABqd3RfYnIjACQAAAA4OWJhOTAzMC1kZDBiLTQyNGItYjkyOC04YTk2MTEwMWZlOGY.eYxNAViy-5qvmyzp-NhvlLTBe9SAWN2nLczqlqTD-J_OpCpkm9BFshcmyiGyruw4Kg8wV-9x008MlS6qJM_kxjnIpXrrfwxID_sbrqyPGPvfs5wvu9wcwn_tr5LuLEgxImbQdktEZeeIe7T3HPEJdvvAS_hgPalcHZitRGw85WqNgyYB2O4Zwtt_YRG_QYPHZWsDlxxxQXs1RJ8-sKhpfrpoalb9uhl_S-KOwpscLVpSl_5vBy-mDtgKyCJKZFFrTFgNsWR5Eu1yPSS19VgJWDXpUH7_0PuPDd4SEREXJa8qn-nd1Bttw0mTD9_LQUQHOiRIBjBksfEafJGrb3NZIg";

        private static string access_token = "eyJ0eXAiOiJNVCIsImFsZyI6IlJTMjU2Iiwia2lkIjoiNjgxODVmZjEtNGU1MS00Y2U5LWFmMWMtNjg5ODEyMjAzMzE3In0.AQoAAAABAAUABwAAntUgauHaSAgAAAaagnLh2kgCABzoxDebUz5LpoL1naYeHrAVAAEAAAAYAAIAAAAFAAAAHQAAAA0AJAAAADg5YmE5MDMwLWRkMGItNDI0Yi1iOTI4LThhOTYxMTAxZmU4ZiIAJAAAADg5YmE5MDMwLWRkMGItNDI0Yi1iOTI4LThhOTYxMTAxZmU4ZhIAAQAAAAYAAABqd3RfYnIjACQAAAA4OWJhOTAzMC1kZDBiLTQyNGItYjkyOC04YTk2MTEwMWZlOGY.w7u_nivI9MTlova5ebkiUFbiFfqsN7FGPSgtR1vEvsjypNFHPT56qJt-N4bi4KdOudnn_drE_Uj2YlZuIerK3SKJWw8PILzAOfEesAgbn4qAqZrsNrxucJPvuLpyl8KCy5Zt4HGEbcw2UmUMJPiVTAxkPfCJU09RMpxw8NtzKUQvQZDKuOs73aybuetdMdn-0o95n3-QOvFI17kSEc5Ch9oscSgEuTpLF6loRl5_y1WaUXpkQ9VtUzYwS_-yRF7UJrF2cHGfepeL2cdz_98nmLtcaBGadyoxbYEt_wfYWi3PNxtWcrChJX0tPsEqM9xw5u6ClvWrjzwErrwMpW8Txg";
        private static string accountId = "89696be1-4771-4b09-83c6-47134d3eaaeb";
        private static string basePath = "https://demo.docusign.net/restapi";
        private static string templateId = "ab68646a-f6de-4f94-af3b-ab35201e0978";
        private static string envelopId = "cd58894d-4c0a-42d9-9087-58bf78016c9e";

        public static string GetToken()
        {
            return String.Format("{0}/oauth/token?grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer&assertion={1}", accountBaseUri, authToken);
        }

        private static EnvelopeTemplate MakeTemplate(string resultsTemplateName)
        {
            // Data for this method
            // resultsTemplateName

            // document 1 (pdf) has tag /sn1/
            //
            // The template has two recipient roles.
            // recipient 1 - signer
            // recipient 2 - cc
            // The template will be sent first to the signer.
            // After it is signed, a copy is sent to the cc person.
            // read file from a local directory
            // The reads could raise an exception if the file is not available!
            // add the documents
            Document doc = new Document();
            string docB64 = Convert.ToBase64String(System.IO.File.ReadAllBytes("World_Wide_Corp_fields.pdf"));
            doc.DocumentBase64 = docB64;
            doc.Name = "Lorem Ipsum"; // can be different from actual file name
            doc.FileExtension = "pdf";
            doc.DocumentId = "1";

            // create a signer recipient to sign the document, identified by name and email
            // We're setting the parameters via the object creation
            Signer signer1 = new Signer();
            signer1.RoleName = "signer";
            signer1.RecipientId = "1";
            signer1.RoutingOrder = "1";

            // routingOrder (lower means earlier) determines the order of deliveries
            // to the recipients. Parallel routing order is supported by using the
            // same integer as the order for two or more recipients.

            // create a cc recipient to receive a copy of the documents, identified by name and email
            // We're setting the parameters via setters
            CarbonCopy cc1 = new CarbonCopy();
            cc1.RoleName = "cc";
            cc1.RoutingOrder = "2";
            cc1.RecipientId = "2";

            // Create fields using absolute positioning:
            SignHere signHere = new SignHere();
            signHere.DocumentId = "1";
            signHere.PageNumber = "1";
            signHere.XPosition = "191";
            signHere.YPosition = "148";

            Checkbox check1 = new Checkbox();
            check1.DocumentId = "1";
            check1.PageNumber = "1";
            check1.XPosition = "75";
            check1.YPosition = "417";
            check1.TabLabel = "ckAuthorization";

            Checkbox check2 = new Checkbox();
            check2.DocumentId = "1";
            check2.PageNumber = "1";
            check2.XPosition = "75";
            check2.YPosition = "447";
            check2.TabLabel = "ckAuthentication";

            Checkbox check3 = new Checkbox();
            check3.DocumentId = "1";
            check3.PageNumber = "1";
            check3.XPosition = "75";
            check3.YPosition = "478";
            check3.TabLabel = "ckAgreement";

            Checkbox check4 = new Checkbox();
            check4.DocumentId = "1";
            check4.PageNumber = "1";
            check4.XPosition = "75";
            check4.YPosition = "508";
            check4.TabLabel = "ckAcknowledgement";

            List list1 = new List();
            list1.DocumentId = "1";
            list1.PageNumber = "1";
            list1.XPosition = "142";
            list1.YPosition = "291";
            list1.Font = "helvetica";
            list1.FontSize = "size14";
            list1.TabLabel = "list";
            list1.Required = "false";
            list1.ListItems = new List<ListItem>
            {
                new ListItem { Text = "Red", Value = "Red" },
                new ListItem { Text = "Orange", Value = "Orange" },
                new ListItem { Text = "Yellow", Value = "Yellow" },
                new ListItem { Text = "Green", Value = "Green" },
                new ListItem { Text = "Blue", Value = "Blue" },
                new ListItem { Text = "Indigo", Value = "Indigo" },
                new ListItem { Text = "Violet", Value = "Violet" },
            };

            // The SDK can't create a number tab at this time. Bug DCM-2732
            // Until it is fixed, use a text tab instead.
            //   , number = docusign.Number.constructFromObject({
            //         documentId: "1", pageNumber: "1", xPosition: "163", yPosition: "260",
            //         font: "helvetica", fontSize: "size14", tabLabel: "numbersOnly",
            //         height: "23", width: "84", required: "false"})
            Text textInsteadOfNumber = new Text();
            textInsteadOfNumber.DocumentId = "1";
            textInsteadOfNumber.PageNumber = "1";
            textInsteadOfNumber.XPosition = "153";
            textInsteadOfNumber.YPosition = "260";
            textInsteadOfNumber.Font = "helvetica";
            textInsteadOfNumber.FontSize = "size14";
            textInsteadOfNumber.TabLabel = "numbersOnly";
            textInsteadOfNumber.Height = "23";
            textInsteadOfNumber.Width = "84";
            textInsteadOfNumber.Required = "false";

            RadioGroup radioGroup = new RadioGroup();
            radioGroup.DocumentId = "1";
            radioGroup.GroupName = "radio1";

            radioGroup.Radios = new List<Radio>
            {
                new Radio { PageNumber = "1", Value = "white", XPosition = "142", YPosition = "384", Required = "false" },
                new Radio { PageNumber = "1", Value = "red", XPosition = "74", YPosition = "384", Required = "false" },
                new Radio { PageNumber = "1", Value = "blue", XPosition = "220", YPosition = "384", Required = "false" },
            };

            Text text = new Text();
            text.DocumentId = "1";
            text.PageNumber = "1";
            text.XPosition = "153";
            text.YPosition = "230";
            text.Font = "helvetica";
            text.FontSize = "size14";
            text.TabLabel = "text";
            text.Height = "23";
            text.Width = "84";
            text.Required = "false";

            // Tabs are set per recipient / signer
            Tabs signer1Tabs = new Tabs();
            signer1Tabs.CheckboxTabs = new List<Checkbox>
            {
                check1, check2, check3, check4,
            };

            signer1Tabs.ListTabs = new List<List> { list1 };

            // numberTabs: [number],
            signer1Tabs.RadioGroupTabs = new List<RadioGroup> { radioGroup };
            signer1Tabs.SignHereTabs = new List<SignHere> { signHere };
            signer1Tabs.TextTabs = new List<Text> { text, textInsteadOfNumber };

            signer1.Tabs = signer1Tabs;

            // Add the recipients to the env object
            Recipients recipients = new Recipients();
            recipients.Signers = new List<Signer> { signer1 };
            recipients.CarbonCopies = new List<CarbonCopy> { cc1 };

            // create the overall template definition
            EnvelopeTemplate template = new EnvelopeTemplate();

            // The order in the docs array determines the order in the env
            template.Description = "Example template created via the API";
            template.Name = resultsTemplateName;
            template.Documents = new List<Document> { doc };
            template.EmailSubject = "Please sign this document";
            template.Recipients = recipients;
            template.Status = "created";

            return template;
        }

        public static (bool createdNewTemplate, string templateId, string resultsTemplateName) CreateTemplate(string accessToken, string basePath, string accountId)
        {
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + access_token);
            TemplatesApi templatesApi = new TemplatesApi(apiClient);
            TemplatesApi.ListTemplatesOptions options = new TemplatesApi.ListTemplatesOptions();
            options.searchText = "Example Signer and CC template";
            EnvelopeTemplateResults results = templatesApi.ListTemplates(accountId);


            string templateId;
            string resultsTemplateName;
            bool createdNewTemplate;

            // Step 2. Process results
            if (int.Parse(results.ResultSetSize) > 0)
            {
                // Found the template! Record its id
                templateId = results.EnvelopeTemplates[0].TemplateId; //ab68646a-f6de-4f94-af3b-ab35201e0978
                resultsTemplateName = results.EnvelopeTemplates[0].Name;
                createdNewTemplate = false;
            }
            else
            {
                // No template! Create one!
                EnvelopeTemplate templateReqObject = MakeTemplate("test1");

                TemplateSummary template = templatesApi.CreateTemplate(accountId, templateReqObject);

                // Retrieve the new template Name / TemplateId
                EnvelopeTemplateResults templateResults = templatesApi.ListTemplates(accountId, options);
                templateId = templateResults.EnvelopeTemplates[0].TemplateId;
                resultsTemplateName = templateResults.EnvelopeTemplates[0].Name;
                createdNewTemplate = true;
            }

            return (createdNewTemplate: createdNewTemplate,
                templateId: templateId, resultsTemplateName: resultsTemplateName);
        }

        public static string SendEnvelopeFromTemplate(string signerEmail, string signerName, string ccEmail, string ccName)
        {
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + access_token);

            

            // Step 2 start
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);
            EnvelopeDefinition envelope = MakeEnvelope(signerEmail, signerName, ccEmail, ccName, templateId);
            EnvelopeSummary result = envelopesApi.CreateEnvelope(accountId, envelope);
            return result.EnvelopeId;
            // Step 2 end
        }

        private static EnvelopeDefinition MakeEnvelope(
            string signerEmail,
            string signerName,
            string ccEmail,
            string ccName,
            string templateId)
        {
            // Data for this method
            // signerEmail
            // signerName
            // ccEmail
            // ccName
            // templateId

            EnvelopeDefinition env = new EnvelopeDefinition();
            env.TemplateId = templateId;

            TemplateRole signer1 = new TemplateRole();
            signer1.Email = signerEmail;
            signer1.Name = signerName;
            signer1.RoleName = "signer111";

            TemplateRole cc1 = new TemplateRole();
            cc1.Email = ccEmail;
            cc1.Name = ccName;
            cc1.RoleName = "cc";

            env.TemplateRoles = new List<TemplateRole> { signer1, cc1 };
            env.Status = "sent";
            return env;
            // Step 3 end
        }

        public static Recipients GetRecipients(string envelopeId)
        {
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + access_token);
            // Step 2 end

            // Step 3 start
            EnvelopesApi envelopesApi = new EnvelopesApi(apiClient);
            Recipients results = envelopesApi.ListRecipients(accountId, envelopeId);
            return results;
            // Step 3 end
        }

        public static List<Checkbox> GetDocCheckboxFields()
        {
            return new List<Checkbox>()
            {
                new Checkbox() {
                    TabLabel="is_trust_beneficiary",
                    Selected = "true"
                },
                new Checkbox() {
                    TabLabel="is_executor",
                    Selected = "true"
                },
                new Checkbox() {
                    TabLabel="is_lessee",
                    Selected = "true"
                },
                new Checkbox() {
                    TabLabel="is_auth_officer",
                    Selected = "true"
                },
                new Checkbox() {
                    TabLabel="is_recent_purchase",
                    Selected = "true"
                },
                new Checkbox() {
                    TabLabel="is_not_recent_purchase",
                    Selected = "true"
                },
                new Checkbox() {
                    TabLabel="is_former_owner",
                    Selected = "true"
                }
            };
        }

        public static List<Text> GetDocTextFields()
        {
            return new List<Text>()
            {
                new Text() {
                    TabLabel="appeal_year",
                    Value="2022"
                },
                new Text() {
                    TabLabel="township_name",
                    Value="Northfield"
                },
                new Text() {
                    TabLabel="formatted_pins_line_1",
                    Value="04-14-100-016-0000, 05-12-234-001-0000"
                },
                new Text() {
                    TabLabel="formatted_pins_line_2",
                    Value="04-14-100-222-0000, 05-12-234-222-0000"
                },
                new Text() {
                    TabLabel="formatted_pins_line_3",
                    Value="04-14-100-333-0000, 05-12-234-333-0000"
                },
                new Text() {
                    TabLabel="formatted_pins_line_3",
                    Value="04-14-100-333-0000, 05-12-234-333-0000"
                },
                new Text() {
                    TabLabel="property_address_line_1",
                    Value=""
                },
                new Text() {
                    TabLabel="property_city",
                    Value=""
                },
                new Text() {
                    TabLabel="property_state",
                    Value=""
                },
                new Text() {
                    TabLabel="property_zipcode",
                    Value=""
                },
                new Text() {
                    TabLabel="company_name",
                    Value=""
                },
                new Text() {
                    TabLabel="purchase_date",
                    Value=""
                },
                new Text() {
                    TabLabel="three_years_ago",
                    Value=""
                },
                new Text() {
                    TabLabel="purchase_price",
                    Value=""
                }
            };
        }

        public static List<TemplateRole> GetTemplateRoles(Tabs tabs)
        {
            return new List<TemplateRole>()
            {
                // Create a signer recipient to sign the document, identified by name and email
                // We're setting the parameters via the object creation
                new TemplateRole()
                {
                    Email = "processing@propertytaxfox.com",
                    Name = "Max Stolyarov",
                    RoleName = "editor",
                    ClientUserId = "1",
                    Tabs = tabs //Set tab values
                },
                new TemplateRole()
                {
                    Email = "stolmax@hotmail.com",
                    Name = "Max Stolyarov",
                    RoleName = "homeowner",
                    Tabs = tabs //Set tab values
                },

                new TemplateRole()
                {
                    Email = "invtech@hotmail.com",
                    Name = "Max Second",
                    RoleName = "attorney",
                    Tabs = tabs //Set tab values
                }
            };
        }

        public static void getTemplateId()
        {
            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + access_token);
            TemplatesApi templatesApi = new TemplatesApi(apiClient);
            EnvelopeTemplateResults results = templatesApi.ListTemplates(accountId);


            string tempId = "";
            string resultsTemplateName;

            // Step 2. Process results
            if (int.Parse(results.ResultSetSize) > 0)
            {
                // Found the template! Record its id
                tempId = results.EnvelopeTemplates[0].TemplateId; //ab68646a-f6de-4f94-af3b-ab35201e0978
                resultsTemplateName = results.EnvelopeTemplates[0].Name;
            }

            Console.WriteLine(tempId);
        }

            static void Main(string[] args)
        {
            /*DocuSignClient client = new DocuSignClient();
            client.SetOAuthBasePath("account-d.docusign.com");
            Account acct = client.GetUserInfo(access_token).Accounts.FirstOrDefault();
            Console.WriteLine(acct.ToString());*/

            //SendEnvelopeFromTemplate("sghsgm99@gmail.com", "oleg", "max@propertytaxfox.com", "max");
            /*Recipients result = GetRecipients(envelopId);

            Console.WriteLine("RecipientCount: {0}\r\n", result.RecipientCount);

            int index = 1;
            result.Signers.ForEach(x => {
                Console.WriteLine("Recipient{0} Name: {1}", index, x.Name);
                Console.WriteLine("Recipient{0} RoleName: {1}", index, x.RoleName);
                Console.WriteLine("Recipient{0} GroupName: {1}", index, x.SigningGroupName);
                Console.WriteLine("Recipient{0} SentDate: {1}", index, x.SentDateTime);
                Console.WriteLine("Recipient{0} Status: {1}", index, x.Status);
                Console.WriteLine("Recipient{0} BulkRecipientsUri: {1}", index, x.BulkRecipientsUri);
                Console.WriteLine("Recipient{0} EmailRecipientPostSigningURL: {1}", index, x.EmailRecipientPostSigningURL);
                Console.WriteLine("Recipient{0} EmbeddedRecipientStartURL: {1}", index, x.EmbeddedRecipientStartURL);
                
                Console.WriteLine("------------------------\r\n");
                index++;
            });*/

            getTemplateId();

            return;


            var apiClient = new DocuSignClient(basePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + access_token);


            Tabs tabs = new Tabs
            {
                CheckboxTabs = GetDocCheckboxFields(),
                RadioGroupTabs = null,
                TextTabs = GetDocTextFields(),
                ListTabs = null
            };

            EnvelopeDefinition envelopeAttributes = new EnvelopeDefinition
            {
                // Uses the template ID received from example 08
                TemplateId = templateId,
                Status = "Sent",
                // Add the TemplateRole objects to utilize a pre-defined
                // document and signing/routing order on an envelope.
                // Template role names need to match what is available on
                // the correlated templateID or else an error will occur
                TemplateRoles = GetTemplateRoles(tabs),
                EmailSubject = "BOR Atty Authorization Form",
                EmailBlurb = "Dear Client,\n\nI hope you are well. I am writing about the property tax appeal we are preparing for your property.  To continue with the appeal, we need to have a Cook County Board of Review Attorney Authorization form on file, signed by you and our attorney, Anthony DeFrenza, from the Law Office of DeFrenza Mosconi PC, see below:\n\n\t1.\tBOR Attorney Authorization Form.\n\nIf this document meets your approval, please follow the prompts from Adobe to electronically sign this document. If you decide to hire another attorney for this appeal to represent you at the Cook County Board of Review, please let us know. Once you sign this document, Anthon DeFrenza will also sign this document as part of this communication. It is our pleasure to assist you in this matter. \n\nPlease do not hesitate to contact me directly with any questions you may have.\n\nSincerely,\n\nMax Stolyarov\nFounder/CEO of Property Tax Fox Corp\nPhone: 847-957-3690\nFax: 847-510-0777\nhttps://propertytaxfox.com\n"
            };

            EnvelopesApi envelopesApi = null;
            EnvelopeSummary results = null;

            try
            {
                envelopesApi = new EnvelopesApi(apiClient);
                results = envelopesApi.CreateEnvelope(accountId, envelopeAttributes);
            }
            catch (DocuSign.eSign.Client.ApiException ex)
            {
                Console.Write(ex.ErrorMessage);
            }

            string envelopeId = results.EnvelopeId;
            string status = results.Status;

            RecipientViewRequest viewRequest = new RecipientViewRequest();

            viewRequest.ReturnUrl = "https://biz-api.propertytaxdetective.com/thank-you"; // dsReturnUrl + "?state=123";

            // How has your app authenticated the user? In addition to your app's authentication,
            // you can include authentication steps from DocuSign; e.g., SMS authentication
            viewRequest.AuthenticationMethod = "none";

            // Recipient information must match the embedded recipient info
            // that we used to create the envelope
            viewRequest.Email = "processing@propertytaxfox.com";
            viewRequest.UserName = "Max Stolyarov";
            viewRequest.ClientUserId = "1";


            ViewUrl results1 = envelopesApi.CreateRecipientView(accountId, results.EnvelopeId, viewRequest);
            //***********
            // Don't use an iframe with embedded signing requests!
            //***********
            // State can be stored/recovered using the framework's session or a
            // query parameter on the return URL (see the makeRecipientViewRequest method)
            string redirectUrl = results1.Url;

            Console.WriteLine(redirectUrl);
        }
    }
}
