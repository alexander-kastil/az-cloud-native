using System;
using System.Collections.Generic;
using Azure.Identity;
using Microsoft.Graph;

namespace FoodApp
{
    public class GraphHelper
    {
        AppConfig config;
        AILogger Logger;

        public GraphHelper(IConfiguration cfg, AILogger ai)
        {
            config = cfg.Get<AppConfig>();
            Logger = ai;
        }

        public void SendMail(string Subject, string Message, string[] Recipient)
        {
            var recipients = new List<Recipient>();

            foreach (var r in Recipient)
            {
                AddRecipient(recipients, r);
            }

            var body = new ItemBody
            {
                ContentType = BodyType.Html,
                Content = Message,
            };

            Message message = new Message
            {
                Subject = Subject,
                Body = body,
                ToRecipients = recipients,
            };

            Logger.LogEvent("SendMail", new Dictionary<string, string> { { "Subject", Subject }, { "Message", Message }, { "Recipient", Recipient[0] } });
            SendGraphMail(config.GraphCfg, message);            
        }       
        private void SendGraphMail(GraphCfg config, Message msg)
        {
            //Get Graph Client
            var graphOptions = new ClientSecretCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            };  
            var clientSecretCredential = new ClientSecretCredential(config.TenantId, config.ClientId, config.ClientSecret, graphOptions);
            var graphClient = new GraphServiceClient(clientSecretCredential);
            
            //POST /users/{id | userPrincipalName}/sendMail
            graphClient.Users[config.MailSender].SendMail(msg, false).Request().PostAsync();                        
        }

        private void AddRecipient(List<Recipient> toRecipientsList, string r)
        {
            var emailAddress = new EmailAddress
            {
                Address = r,
            };

            var toRecipients = new Recipient
            {
                EmailAddress = emailAddress,
            };
            toRecipientsList.Add(toRecipients);
        }
    }
}