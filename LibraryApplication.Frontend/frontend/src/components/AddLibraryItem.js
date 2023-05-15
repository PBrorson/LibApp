import React, { useRef, useState, useEffect } from "react";
import Form from "react-bootstrap/Form";
import Button from "react-bootstrap/Button";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { LibraryItemType } from "./LibraryItemType";

function AddLibraryItem() {
  const [libraryItems, setLibraryItems] = useState([]);
  const titleRef = useRef("");
  const authorRef = useRef("");
  const pagesRef = useRef("");
  const runTimeMinutesRef = useRef("");
  const categoryRef = useRef("");
  const borrowerRef = useRef("");
  const [error, setError] = useState(null);
  const [categories, setCategories] = useState([]);
  const [selectedType, setSelectedType] = useState("");
  const navigate = useNavigate();

  function addLibraryItemHandler() {
    const borrower = borrowerRef.current.value;
    if (!borrower) {
      setError(<h3>Borrower name is required!</h3>);
      return;
    }

    const title = titleRef.current.value;

    const existingItem = libraryItems.find(
      (item) => item.title === title && !item.isBorrowable
    );
    if (existingItem) {
      setError("A library item with the same title already exists and is not borrowable.");
      return;
    }
    const payload = {
      title: titleRef.current.value,
      category: { categoryName: categoryRef.current.value },
      title: titleRef.current.value,
      author: authorRef.current.value,
      pages: parseInt(pagesRef.current.value),
      runTimeMinutes: parseInt(runTimeMinutesRef.current.value),
      borrower: borrowerRef.current.value,
      borrowDate: new Date().toISOString(),
      dueDate: new Date().toISOString(),
      type: 1,
      titleAcronym: titleRef.current.value.substring(0, 1).toUpperCase(),
    };
    

    axios
      .post("https://localhost:7282/CreateItems", payload)
      .then((response) => {
        navigate("/");
      })
      .catch((error) => {
        if (error.response && error.response.data) {
          setError(error.response.data);
        } else {
          setError("Error creating library item");
        }
        console.log(error.response);
      });
  }
  useEffect(() => {
    axios
      .get("https://localhost:7282/api/Categories")
      .then((response) => {
        setCategories(response.data);
      })
      .catch((error) => {
        console.error(error);
      });
  }, []);
  
  return (
    <>
      <legend>
        <b>Add Library Item</b>
      </legend>

      {error && <div className="error">{error}</div>}

      <Form>
        <Form.Group className="mb-3" controlId="formTitle">
          <Form.Label>Title</Form.Label>
          <Form.Control type="text" ref={titleRef} required />
        </Form.Group>

        <Form.Group className="mb-3" controlId="formAuthor">
          <Form.Label>Author</Form.Label>
          <Form.Control type="text" ref={authorRef} required />
        </Form.Group>

        <Form.Group className="mb-3" controlId="formPages">
          <Form.Label>Number of Pages</Form.Label>
          <Form.Control type="number" ref={pagesRef} min="1" />
        </Form.Group>

        <Form.Group className="mb-3" controlId="formRunTimeMinutes">
          <Form.Label>Run Time (Minutes)</Form.Label>
          <Form.Control type="number" ref={runTimeMinutesRef} min="1" />
        </Form.Group>

        <Form.Group className="mb-3" controlId="formCategory">
  <Form.Label>Category</Form.Label>
  <Form.Select ref={categoryRef} required>
    {categories.map((category) => (
      <option key={category.id} value={category.categoryName}>
        {category.categoryName}
      </option>
    ))}
  </Form.Select>
</Form.Group>
        <Form.Group className="mb-3" controlId="formType">
          <Form.Label>Type</Form.Label>
          <Form.Select
            value={selectedType}
            onChange={(e) => setSelectedType(parseInt(e.target.value))}
            required
          >
            <option value={LibraryItemType.Book}>Book</option>
            <option value={LibraryItemType.ReferenceBook}>
              Reference Book
            </option>
            <option value={LibraryItemType.DVD}>DVD</option>
            <option value={LibraryItemType.AudioBook}>Audio Book</option>
          </Form.Select>
        </Form.Group>

        <Form.Group className="mb-3" controlId="formBorrower">
          <Form.Label>Borrower</Form.Label>
          <Form.Control type="text" ref={borrowerRef} />
        </Form.Group>

        <Button variant="success" type="button" onClick={addLibraryItemHandler}>
          Submit Library Item
        </Button>
      </Form>
    </>
  );
}

export default AddLibraryItem;
