using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Generic_Text_Generator
{
    /// <summary>
    /// This utility provides the ability for a user to quickly write canned-data emails. They can define
    /// their templates in a file called 'FormTemplates.txt', which will be read in and accessible to edit.
    /// These templates contain variables, which the user can edit via text boxes in order to fill out the email.
    /// The user can provide their email informatio in order to send this form directly from the utility or they can
    /// copy the form to their clipboard via button press.
    /// 
    /// By Tracey Heath (2015)
    /// </summary>
    public partial class MainWindow : Window
    {
        //Create 3 dictionaries for use within the program. 
        // The map of templates, with their contents
        // The map of variables in the current template
        // The map of email hosts available
        Dictionary<string, string> templateList = new Dictionary<string, string>();
        Dictionary<string, string> templateVars = new Dictionary<string, string>();
        Dictionary<string, string> availableHosts = new Dictionary<string, string>();

        string currentTemplate;
        string hostName = "smtp.gmail.com";

        public MainWindow()
        {
            InitializeComponent();
            readSMTPHosts();
            readTemplates();
        }

        // Method to read host options from the 'EmailHosts.txt' file
        private void readSMTPHosts()
        {
            string currentLine = "";
            string[] currentHost;
            //start default host as true, since the 1st value found is the default
            bool defaultHost = true;
            bool error = false;
            if (File.Exists("EmailHosts.txt"))
            {
                StreamReader sr = new StreamReader("EmailHosts.txt");
                while ((currentLine = sr.ReadLine()) != null)
                {
                    //remove all whitespace from the line
                    currentLine = string.Join("", currentLine.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                    if (currentLine == "" || currentLine[0] == '#')
                    {
                        continue;
                    }

                    currentHost = currentLine.Split(',');
                    if (currentHost.Length < 2)
                    {
                        error = true;
                        continue;

                    }

                    MenuItem newService = new MenuItem();
                    newService.Header = currentHost[0];
                    newService.IsCheckable = true;

                    if(defaultHost)
                    {
                        hostName = currentHost[1];
                        newService.IsChecked = true;
                        defaultHost = false;
                    }

                    newService.Checked += newServiceChecked;
                    serviceMenu.Items.Add(newService);
                    availableHosts.Add(currentHost[0], currentHost[1]);

                }
                if (error)
                {
                    MessageBox.Show("Error: emailHosts file incorrect. Hosts must be of format [serice name],[smtpAddress]");
                }

            }
            else
            {
                MessageBox.Show("Email host file not found. Please check containing folder for the file \"emailHosts.txt\"");
            }

        }

        // Method to read templates from the 'FormTemplates.txt' file
        private void readTemplates()
        {
            string currentLine = "";
            string title = "";
            string template = "";
            bool templateBeginning = true;
            if (File.Exists("FormTemplates.txt"))
            {
                StreamReader sr = new StreamReader("FormTemplates.txt");

                while ((currentLine = sr.ReadLine()) != null)
                {
                    if (templateBeginning == true)
                    {
                        title = currentLine;
                        template = "";
                        templateBeginning = false;
                    }
                    else if (currentLine[0] == '~')
                    {
                        templateList.Add(title, template);
                        templateBeginning = true;
                    }
                    else
                    {
                        template += currentLine + '\n';
                    }
                }
            }
            else
            {
                MessageBox.Show("Template data not found. Please check containing folder for the file \"FormTemplates.txt\"");
            }

            foreach (string key in templateList.Keys.ToList())
            {
                formSelection.Items.Add(key);
            }
            formSelection.SelectedIndex = 0;
        }

        // Method to read in the form variables from a template and populate the template variables dictionary
        private void getInitialFormVariables()
        {
            bool multiWord = false;
            string varName = "";

            foreach (string word in currentTemplate.Split(' ', '\n'))
            {
                if (word.CompareTo("") == 0)
                {
                    continue;
                }
                if (!multiWord)
                {
                    varName = "";
                }
                if (word[0] == '[' || multiWord == true)
                {
                    if (word.Contains(']'))
                    {
                        varName += word;
                        templateVars.Add(varName.Substring(1, varName.IndexOf(']') - 1), varName.Substring(0, varName.IndexOf(']') + 1));
                        multiWord = false;
                    }
                    else
                    {
                        varName += word + " ";
                        multiWord = true;
                    }

                }
            }
        }

        #region Event Handlers

        // Event handler for clicking the 'Send Email' button.
        // Takes the data and sends it with the information entered in the boxes
        private void emailData_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                SmtpClient smtpmail = new SmtpClient();
                smtpmail.Host = hostName;
                smtpmail.Port = 587;
                smtpmail.EnableSsl = true;
                smtpmail.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpmail.UseDefaultCredentials = false;
                smtpmail.Credentials = new NetworkCredential(uname.Text, pword.Password);
                MailMessage message = new MailMessage(uname.Text, custEmail.Text, subjectLine.Text, currentText.Text);
                message.IsBodyHtml = true;
                smtpmail.Send(message);
                MessageBox.Show("Email Sent");

                //untested code for attachments
                /*if (ListBox1.Items.Count != 0)
                {
                    for (i = 0; i < ListBox1.Items.Count; i++)
                        mail.Attachments.Add(new Attachment(ListBox1.Items[i].ToString()));
                }*/
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // Event handler for the 'Copy to Clipboard' button.
        // Copies the text from the template box to the clipboard.
        private void copyData_Click_1(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(currentTemplate);
        }

        // Event handler for when a template is selected via the drop-down menu
        // Uses the templateList dictionary to grab the form data and populate the utility
        private void formSelection_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            currentTemplate = templateList[(string)formSelection.SelectedValue];
            subjectLine.Text = (string)formSelection.SelectedValue;

            //Clear the template variables, since it has changed
            templateVars.Clear();

            currentText.Text = currentTemplate;
            getInitialFormVariables();

            inputPanel.Children.Clear();
            foreach (string key in templateVars.Keys.ToList())
            {
                //replace all space in keys with underscores, so they can be used as variables to access the key
                string temp = key.Replace(' ', '_');
                Label lb = new Label();
                lb.Content = key + ": ";
                TextBox tb = new TextBox();
                tb.Name = temp;
                tb.LostFocus += setFormVariables;
                tb.Width = 300;
                tb.Height = 25;

                inputPanel.Children.Add(lb);
                inputPanel.Children.Add(tb);
            }
        }

        // Event handler for when the user changes focus from a variable textbox
        // Inserts the textbox value into the form below
        private void setFormVariables(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            string boxKey = tb.Name;
            //Return the underscores to spaces in order to find the keys in the template
            boxKey = boxKey.Replace('_', ' ');
            string newValue = tb.Text;

            //If the textbox is empty, set the value to the original key
            if (newValue.CompareTo("") == 0)
            {
                newValue = templateVars[boxKey];
            }
            currentTemplate = currentTemplate.Replace(templateVars[boxKey], newValue);
            templateVars[boxKey] = newValue;

            currentText.Text = currentTemplate;
        }

        // Generic event handler for the email service menu items
        // Takes the sender and sets it as the new host, 
        // then goes through and ensures that no other hosts are checked
        private void newServiceChecked(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            hostName = availableHosts[(string)mi.Header];

            foreach (MenuItem item in serviceMenu.Items)
            {
                if (item.IsChecked && item.Header != mi.Header)
                {
                    item.IsChecked = false;
                }
            }
        }

        private void HelpMenu_Click_1(object sender, RoutedEventArgs e)
        {
            MenuItem clicked = sender as MenuItem;
            switch ((string)clicked.Header)
            {
                case "General Overview":
                    MessageBox.Show("This is a utility designed to allow the user to use templates in order to draft generic emails.\n\n"+
                                    "Templates are defined by the user, with variable pieces and static text. "+
                                    "They can be futher edited on loading within the utility.\n\n"+
                                    "Created By Tracey Heath (2015)",
                                    "General Overview");
                    break;
                case "Templates":
                    MessageBox.Show("Templates are defined in the file 'FormTemplates.txt'.\n\n " +
                                    "In this file, templates have 4 distinct elements and all must be used to work.\n\n"+
                                    "1. The title, which is the first line of the template. This is also the default email subject.\n"+
                                    "2. he static text, which is anything outside of [] brackets between the title line and terminator line.\n"+
                                    "3. The variable items, which are keys interspersed within the static text, surrounded by [] brackets.\n"+
                                    "4. The terminator, which is the ~ symbol on it's own line, following the template definition.\n\n"+
                                    "Example:\n\tThis is the title.\n\tThis is the message, and the changing items will appear like [this].\n\t~",
                                    "Templates");
                    break;
                case "Email Service":
                    MessageBox.Show("Email services are defined in the file 'EmailHosts.txt'.\n\n"+
                                    "By default, this is set to GMail. Other common services are already provided as options. "+
                                    "If you wish to expand this list, you can do so by editing the 'EmailHosts.txt' file. These entries "+
                                    "should be input as [service name],[smtp_address].\n\nExample: \n\tHotmail,smtp.live.com\n\n"+
                                    "They can then be selected via the 'Email Service' menu. This should match the address you are using to send the email.\n\n"+
                                    "Note:\n\n-This application uses port 587 to send mail. This is the most common port to use, but other services may use another.\n\n"+
                                    "-Some services may require additional steps to be done to work with this utility.\n"+
                                    "   E.g. To use gmail you must set 'Allow less secure apps' to ON in your email.\n   (See readme for details.)",
                                    "Email Services");
                    break;
                default:
                    break;
            }
        }
    }
        #endregion
}
