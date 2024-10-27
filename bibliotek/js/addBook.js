$(document).ready(function() {
    const urlParams = new URLSearchParams(window.location.search);
    const bookId = urlParams.get('bookId');

    if (bookId) {
        $.ajax({
            url: `https://localhost:7080/api/Book/${bookId}`,
            type: 'GET',
            dataType: 'json',
            success: function(book) {
                $('#bookId').val(book.id);
                $('#title').val(book.title);
                $('#description').val(book.description);
                $('#year').val(book.year);
                $('#authorId').val(book.authorId);
            },
            error: function(xhr, status, error) {
                console.error('Error fetching book data:', error);
                alert('Could not fetch book data for editing.');
            }
        });
    }

    $('#bookForm').submit(function(event) {
        event.preventDefault();
        
        const bookId = $('#bookId').val().trim();
        const bookData = {
            title: $('#title').val(),
            description: $('#description').val(),
            year: parseInt($('#year').val()),
            authorId: parseInt($('#authorId').val())
        };

        if (bookId) {
            bookData.id = parseInt(bookId); // Inkluder id i bookData ved oppdatering
            updateBook(bookId, bookData);
        } else {
            addBook(bookData);
        }
    });

    function addBook(bookData) {
        console.log("Adding book with data:", bookData);
        
        $.ajax({
            url: 'https://localhost:7080/api/Book',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(bookData),
            success: function(response) {
                alert("Book added successfully!");
                window.location.href = 'index.html';
            },
            error: function(xhr, status, error) {
                console.error('Error adding book:', error);
                console.error('Response status:', xhr.status);
                let errorMessage = 'Failed to add the book.';
                
                if (xhr.responseJSON && xhr.responseJSON.errors) {
                    const errors = xhr.responseJSON.errors;
                    errorMessage += '\n' + Object.values(errors).flat().join('\n');
                }
                
                alert(errorMessage);
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
                window.location.href = 'index.html';
            },
            error: function(xhr, status, error) {
                console.error('Error updating book:', error);
                console.error('Response status:', xhr.status);
                console.error('Full response:', xhr.responseText);
                
                let errorMessage = 'Failed to update the book.';
                
                if (xhr.responseJSON && xhr.responseJSON.errors) {
                    const errors = xhr.responseJSON.errors;
                    errorMessage += '\n' + Object.values(errors).flat().join('\n');
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
