$(document).ready(function() {
    // Get the bookId from the URL if available
    const urlParams = new URLSearchParams(window.location.search);
    const bookId = urlParams.get('bookId');

    if (bookId) {
        // Fetch the book data for editing
        $.ajax({
            url: `https://localhost:7080/api/Book/${bookId}`,
            type: 'GET',
            dataType: 'json',
            success: function(book) {
                $('#bookId').val(book.id); // Populate bookId (hidden field)
                $('#title').val(book.title);
                $('#description').val(book.description);
                $('#year').val(book.year);
                $('#authorId').val(book.authorId);
            },
            error: function(xhr, status, error) {
                console.error('Error fetching book data:', error);
            }
        });
    }

    $('#bookForm').submit(function(event) {
        event.preventDefault();

        const bookId = $('#bookId').val().trim(); // Empty for new books
        const bookData = {
            id: bookId ? parseInt(bookId) : null,  // Include id for update
            title: $('#title').val(),
            description: $('#description').val(),
            year: parseInt($('#year').val()),
            authorId: parseInt($('#authorId').val())
        };

        if (bookId) {
            // Update the existing book
            updateBook(bookId, bookData);
        } else {
            // Add a new book
            addBook(bookData);
        }
    });

    function addBook(bookData) {
        $.ajax({
            url: 'https://localhost:7080/api/Book',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(bookData),
            success: function(response) {
                alert("Book added successfully!");
                window.location.href = 'index.html'; // Redirect to main page
            },
            error: function(xhr, status, error) {
                console.error('Error adding book:', error);
                alert("Failed to add the book.");
            }
        });
    }

    function updateBook(bookId, bookData) {
        console.log("Updating book with ID:", bookId);
        console.log("Book data being sent:", bookData);
    
        $.ajax({
            url: `https://localhost:7080/api/Book/${bookId}`,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(bookData),
            success: function(response) {
                alert("Book updated successfully!");
                window.location.href = 'index.html'; // Redirect to main page
            },
            error: function(xhr, status, error) {
                console.error('Error updating book:', error);
                console.error('Response status:', xhr.status);
                
                let errorMessage = 'Failed to update the book.';
                try {
                    const responseText = JSON.parse(xhr.responseText);
                    if (responseText && responseText.title) {
                        errorMessage += ` Error: ${responseText.title}`;
                    }
                } catch (e) {
                    errorMessage += ' Please check the server log for more details.';
                }

                alert(errorMessage);
            }
        });
    }

    function clearForm() {
        $('#bookId').val('');
        $('#title').val('');
        $('#description').val('');
        $('#year').val('');
        $('#authorId').val('');
    }
});
