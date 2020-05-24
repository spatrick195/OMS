$(function () {
    $('#add-user-button').click(function () {
        // how many users are there
        var userCount = $('.userContainer').length;

        // this count is used for the index for the next user container
        var nextIndex = userCount;
        var html =
            `<div class="userContainer row">
               <div class="form-group mx-1">
                  <label for="email">Email</label>
                  <input id="email" type="text" name="UserToRegister[` + nextIndex + `].Email"  class="form-control" placeholder="Email" />
               </div>
               <div class="form-group mx-1">
                  <label for="password">Password</label>
                  <input id="password" type="password" name="UserToRegister[` + nextIndex + `].Password"  class="form-control" placeholder="Password" />
               </div>
               <div class="form-group">
                  <label for="remove-user-button">Remove</label>
                  <button type="button" class="btn btn-danger form-control" id="remove-user-button"><i class="fas fa-times center"></i></button>
               </div>
            </div>`;

        // append the html
        $('.count').text(userCount);
        $('.users-to-register').append(html);
    });

    //$("#remove-user-button").click(function () {
    //    // make sure user is not togglin
    //    if (!userCount == 0 && !nextIndex == 0) {
    //        userCount-- // decrement userCount
    //        nextIndex--; // decrement nextIndex
    //        $(".userContainer").remove();
    //        $('.count').text(userCount); // update user count text
    //    }
    //});
});