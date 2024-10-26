$(document).ready(function() {
    // Existing fetchData button click event
    $('#fetchData').click(function() {
        $.ajax({
            url: 'https://localhost:7080/api/Book',
            type: 'GET',
            dataType: 'json',
            success: function(data) {
                console.log("Data received:", data);
                $('#dataContainer').text(JSON.stringify(data));
                displayBooks(data);
            },
            error: function(xhr, status, error) {
                console.error('Error fetching data:', error);
            }
        });
    });

    // New postData button click event for adding a book
    $('#postData').click(function() {
        const newBook = {
            title: $('#title').val(),
            description: $('#description').val(),
            year: parseInt($('#year').val()),
            authorId: parseInt($('#authorId').val())
        };

        $.ajax({
            url: 'https://localhost:7080/api/Book',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(newBook),
            success: function(response) {
                console.log("Book added successfully:", response);
                $('#fetchData').click(); // Refresh the list after adding a new book
                clearFormFields();
            },
            error: function(xhr, status, error) {
                console.error('Error adding book:', error);
                console.error('Response status:', xhr.status);
                console.error('Response text:', xhr.responseText);
            }
        });
    });

    function displayBooks(data) {
        const container = $('#dataContainer');
        container.empty(); // Clear previous content
    
        const books = data.$values || data; // Handle JSON structure
    
        books.forEach(book => {
            const bookElement = $(`
                <div class="book">
                    <h2>${book.title}</h2>
                    <p><strong>Description:</strong> ${book.description}</p>
                    <p><strong>Year:</strong> ${book.year}</p>
                    <p><strong>Author ID:</strong> ${book.authorId}</p>
                    <button class="delete-btn" data-id="${book.id}">Delete</button>
                    <button class="update-btn" onclick="window.location.href='addBook.html?bookId=${book.id}'">Update</button>
                </div>
            `);
            container.append(bookElement);
        });
    
        // Attach delete functionality to delete buttons
        $('.delete-btn').click(function() {
            const bookId = $(this).data('id'); // Get bookId from data-id attribute
            deleteBook(bookId);
        });
    }
    
    
    function deleteBook(bookId) {
        $.ajax({
            url: `https://localhost:7080/api/Book/${bookId}`,
            type: 'DELETE',
            success: function() {
                console.log(`Book with ID ${bookId} deleted`);
                $('#fetchData').click();
            },
            error: function(xhr, status, error) {
                console.error('Error deleting book:', error);
                console.error('Response status:', xhr.status);
                console.error('Response text:', xhr.responseText);
            }
        });
    }

    function clearFormFields() {
        $('#title').val('');
        $('#description').val('');
        $('#year').val('');
        $('#authorId').val('');
    }
});
