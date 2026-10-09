function getEmails() {
    var options =
    {
        url: ResolveUrl("~/home/emails"),
        type: "POST",
        data: { __RequestVerificationToken: $("#__RequestVerificationToken").val() },
        success: function (status) {
            var emailsHtml = "";
            $.each(status, function (index, item) { emailsHtml += $("<div>").text(item).html() + "<br/>"; });
            $("#emailData").html(emailsHtml);
        },
        error: function (info) {
            alert(info);
        }
    };

    $.ajax(options);
}