$(document).ready(function () {
    $("#user-@Model.Id .user-box-controls button")
        .click(function () {
            var self = this;
            if ($(this).text() == "Unfollow") {
                console.log("yes");
                $.ajax({
                    url: '@Url.Action("Unfollow", "Users", new { name = Model.Name})',
                    success: function() {
                        $(self).text("Follow");
                    }
                });
            } else {
                $.ajax({
                    url: '@Url.Action("Follow", "Users", new { name = Model.Name})',
                    success: function() {
                        $(self).text("Unfollow");
                    }
                });
            }
        });
})