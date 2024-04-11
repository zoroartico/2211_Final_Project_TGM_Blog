// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function toggleCheckbox(role) {
    console.log(`Checkbox for role ${role} clicked`);
    var checkbox = document.getElementById(`chk_${role}`);
    checkbox.checked = !checkbox.checked;
}
