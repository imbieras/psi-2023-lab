let userList = document.getElementById("userList");
let searchInput = document.getElementById("searchInput");

searchInput.addEventListener("input", function () {
    let searchTerm = searchInput.value.toLowerCase(); // Convert to lowercase for case-insensitive search

    // Loop through each user list item
    Array.from(userList.children).forEach(function (userListItem) {
        let userName = userListItem.querySelector(".user-name").textContent.toLowerCase(); // Adjust the selector based on your HTML structure

        // Check if the user's name contains the search term
        if (userName.includes(searchTerm)) {
            userListItem.style.display = "block"; // Display the user if the search term matches
        } else {
            userListItem.style.display = "none"; // Hide the user if the search term does not match
        }
    });
});
