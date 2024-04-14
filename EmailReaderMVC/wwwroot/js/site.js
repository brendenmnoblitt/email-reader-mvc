document.addEventListener('DOMContentLoaded', function () {
    // Attach click event listener to email items
    var emailItems = document.querySelectorAll('.list-group-item');
    emailItems.forEach(function (emailItem) {
        emailItem.addEventListener('click', function () {
            var emailId = this.getAttribute('data-email-id');
            displayEmailBody(emailId);
        });
    });
});

function displayEmailBody(emailId) {
    // Find the corresponding email details by email ID
    var emailDetails = getEmailDetails(emailId);

    // Update the subject and sender name elements
    document.getElementById('email-subject').textContent = emailDetails.subjectLine;
    document.getElementById('email-sender-name').textContent = emailDetails.senderName;

    // Create an iframe element
    var iframe = document.createElement('iframe');
    iframe.setAttribute('sandbox', 'allow-same-origin allow-scripts');
    // Add any other sandboxing restrictions as needed

    iframe.style.width = '100%'; // Adjust as needed
    iframe.style.height = '80vh'; // Adjust as needed

    // Construct email HTML content
    var emailContent = '<html><head><style>';
    // Add email CSS styles here if needed
    emailContent += '</style></head><body>';
    emailContent += emailDetails.messageBodyHTML; // Assuming messageBodyHTML contains the email content
    emailContent += '</body></html>';

    // Set iframe content with email HTML
    iframe.srcdoc = emailContent;

    // Clear existing email body content
    var emailBody = document.getElementById('email-body');
    emailBody.innerHTML = '';

    // Append iframe to email body
    emailBody.appendChild(iframe);
}


function getEmailDetails(emailId) {
    // Find the email details with the matching email ID
    //console.log("Email to find: " + emailId);
    return emailDetailsList.find(function (emailDetails) {
        //console.log(emailDetails.messageID);
        return emailDetails.messageID == emailId;
    });
}
