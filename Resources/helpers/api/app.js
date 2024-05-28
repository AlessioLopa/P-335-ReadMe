const express = require("express");
const Sequelize = require("sequelize");
const fs = require("fs");
const app = express();
const port = 3000;

const sequelize = new Sequelize("db_ReadMe", "root", "root", {
  host: "localhost",
  port: 6033,
  dialect: "mysql",
});

const Book = sequelize.define("Book", {
  id: {
    type: Sequelize.UUID,
    defaultValue: Sequelize.UUIDV4,
    primaryKey: true,
  },

  title: {
    type: Sequelize.STRING,
    allowNull: false,
  },
  epub: {
    type: Sequelize.BLOB("long"),
  },
});

// Route to serve EPUB files from file system
// app.get("/epub/1", function (req, res) {
//   const file = `${__dirname}/Dickens, Charles - Oliver Twist.epub`;
//   res.download(file);
// });

// Route to serve EPUB files from database
// app.get("/epub/2", function (req, res) {
//   Book.findOne({
//     where: {
//       title: "Oliver Twist",
//     },
//   })
//     .then((book) => {
//       if (book) {
//         res
//           .header("Content-Type", "application/epub+zip")
//           .header(
//             "Content-Disposition",
//             'attachment; filename="' + book.title + '.epub"'
//           )
//           .header("Content-Length", book.epub.length)
//           .send(book.epub);
//       } else {
//         res.status(404).send("Book not found");
//       }
//     })
//     .catch((err) => {
//       console.error("Error:", err);
//       res.status(500).send("Internal Server Error");
//     });
// });

app.get("/epub/all", function (req, res) {
  Book.findAll()
    .then((books) => {
      if (books && books.length > 0) {
        const booksList = books.map((book) => ({
          id: book.id,
          title: book.title,
          author: book.author,
        }));

        res.status(200).json(booksList);
      } else {
        res.status(404).send("No books found");
      }
    })
    .catch((err) => {
      console.error("Error:", err);
      res.status(500).send("Internal Server Error");
    });
});

// Route to serve a specific EPUB file by ID
app.get("/epub/:id", function (req, res) {
  const bookId = req.params.id;

  Book.findByPk(bookId)
    .then((book) => {
      if (book) {
        res
          .header("Content-Type", "application/epub+zip")
          .header(
            "Content-Disposition",
            'attachment; filename="' + book.title + '.epub"'
          )
          .header("Content-Length", book.epub.length)
          .send(book.epub);
      } else {
        res.status(404).send("Book not found");
      }
    })
    .catch((err) => {
      console.error("Error:", err);
      res.status(500).send("Internal Server Error");
    });
});

app.listen(port, () => {
  console.log(`Server listening on port ${port}`);
  // sequelize
  //   .authenticate()
  //   .then(() => {
  //     console.log("Connected to the database");
  //   })
  //   .catch((err) => {
  //     console.error("Unable to connect to the database:", err);
  //   });
  // Book.sync()
  //   .then(() => {
  //     console.log("Book model synchronized with database");
  //     // Insert additional books into the database
  //     const books = [
  //       {
  //         title: "A Christmas Carol",
  //         path: "Dickens, Charles - A Christmas Carol.epub",
  //       },
  //       {
  //         title: "Les trois mousquetaires",
  //         path: "Dumas, Alexandre - Les trois mousquetaires.epub",
  //       },
  //       { title: "Fables", path: "La Fontaine, Jean de - Fables.epub" },
  //       {
  //         title: "Le tour du monde en quatre-vingts jours",
  //         path: "Verne, Jules - Le tour du monde en quatre-vingts jours.epub",
  //       },
  //       {
  //         title: "Sherlock Holmes",
  //         path: "Doyle, Artur Conan - Sherlock Holmes.epub",
  //       },
  //       {
  //         title: "Olivier Twist",
  //         path: "Dickens, Charles - Oliver Twist.epub",
  //       },
  //     ];

  //     books.forEach((book) => {
  //       const epubPath = `${__dirname}/${book.path}`;
  //       const epubData = fs.readFileSync(epubPath);
  //       Book.create({
  //         title: book.title,
  //         epub: epubData,
  //       })
  //         .then(() => {
  //           console.log(`Inserted ${book.title} into the database`);
  //         })
  //         .catch((err) => {
  //           console.error(
  //             `Failed to insert ${book.title} into the database:`,
  //             err
  //           );
  //         });
  //     });
  //   })
  //   .catch((err) => {
  //     console.error("Unable to synchronize book model:", err);
  //   });
});
