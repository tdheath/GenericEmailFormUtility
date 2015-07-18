~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

     Generic Form Email Utility

     Created by Tracey Heath (2015)

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

--------------------------------------
			Description
--------------------------------------

This is a utility designed to allow the user to use templates in order to draft generic emails.It uses an SSL connection in order to
anonymously access the given account and send the email. 

Templates are defined by the user in the 'FormTemplates.txt' file. The utility will
read in a template and populate text boxes based on the variable field found within it. This allows the user to use generic
forms and only edit the unique details as they arise, rather than having to do so within the text. For further versatility,
the text is also displayed in an editable state in the event that a more customized form is called for. All templates must be of
the form:

	title
	text with [variables].
	~

A sample can be found below.

In the top pane of the utility the user can enter their email information as well as that of the intended recipient in order to send
the form to them after it is generated without having to leave the application. The service must be selected and match the user's email
address in order for this to work. This can be done via the 'Email Service' menu item. These services are defined within the 'EmailHosts.txt'
file. New services can be added but take note that some will require additional steps within your email account in order to function. 
By default, Hotmail is selected, but this can be changed within the file as well. Entries in the file must be of the form:

	name_to_appear_in_application,smtp_address

Samples of how these are entered can be found below.


--------------------------------------
			Samples
--------------------------------------
	Template-
		Sample Form
		This is a sample form. The first line is read as the title of the form and is set as the default subject line for
		the email. For a form item,put it within the text with square brackets.For a form element called 'sample', 
		it can be put in like this: [sample]. Be sure to end each form with a tilde. This allows the reader to differentiate 
		between each form.
		~
	Email Host-
		Hotmail, smtp.live.com

--------------------------------------
			Misc Information
--------------------------------------

Be aware that this utility is untested will the majority of email services. 
Only Hotmail, Yahoo, and Google have been confirmed to work at this time.

All email services are done via port 587, which appears to be the most common for email but may not be
compatible with all services.

To set up gmail to work with this utility:
	1. Sign in to gmail
	2. Go to settings
	3. Click 'Accounts and Import' tab
	4. In the 'Change Account Settings' row, click 'Other Google Account Settings'
	5.Under the 'Sign in & security' header, click 'Connected apps & sites.
	6. Within that section, set 'Allow less secure apps' to ON
	
	Note: This means that third party applications are now allowed to access your gmail account. Be aware that opening this avenue can have consequences, should you be cavalier with your account information.