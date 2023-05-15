import React, { useRef, useEffect, useState } from "react";
import Form from "react-bootstrap/Form";
import Button from "react-bootstrap/Button";
import axios from "axios";
import { useNavigate, useParams } from "react-router-dom";
import { LibraryItemType } from "./LibraryItemType";

function UpdateLibraryItem() {
  const [categories, setCategories] = useState([]);
  const titleRef = useRef("");
  const authorRef = useRef("");
  const pagesRef = useRef("");
  const typeRef = useRef("");
  const runTimeMinutesRef = useRef("");
  const categoryRef = useRef("");
  const borrowerRef = useRef("");
  const { id } = useParams();
  const navigate = useNavigate();
  const [item, setItem] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    axios
      .get(`https://localhost:7282/api/LibraryItems/GetItemsById/${id}`)
      .then((response) => {
        const itemData = response.data;
        setItem(itemData);
        setIsLoading(false);
      })
      .catch((error) => {
        console.error("Error retrieving library item:", error);
        setIsLoading(false);
      });
  }, [id]);
  useEffect(() => {
    axios
      .get('https://localhost:7282/api/Categories') // 
      .then((response) => {
        setCategories(response.data);
      })
      .catch((error) => {
        console.error('Error retrieving categories:', error);
      });
  }, []);

  function updateLibraryItemHandler() {

    const payload = {
      id: item.id, 
      categoryId: item.categoryId,
      category: {
        id: item.category.id,
        categoryName: item.category.categoryName,
      },
      title: titleRef.current.value,
      author: authorRef.current.value,
      pages: parseInt(pagesRef.current.value),
      runTimeMinutes: parseInt(runTimeMinutesRef.current.value),
      isBorrowable: item.isBorrowable,
      borrower: borrowerRef.current.value,
      borrowDate: item.borrowDate,
      dueDate: item.dueDate,
      type: parseInt(typeRef.current.value),
      titleAcronym: item.titleAcronym,
      
    };

    console.log("Payload:", payload);
    axios
      .put(
        `https://localhost:7282/api/LibraryItems/UpdateItemsById/${id}`,
        payload
      )
      .then((response) => {
        console.log("Update response:", response.data);
        navigate("/");
      })
      .catch((error) => {
        console.error("Error updating library item:", error);
      });
  }
  if (isLoading || !item) {
    return <div>Loading...</div>;
  }

  return (
    <>
      <legend>
        <b>Update Library Item</b>
      </legend>
      <Form>
        {/* Form fields */}
        {item && (
          <>
            <Form.Group className="mb-3" controlId="formTitle">
              <Form.Label>Title</Form.Label>
              <Form.Control
                type="text"
                ref={titleRef}
                defaultValue={item.title}
                required
              />
            </Form.Group>

            <Form.Group className="mb-3" controlId="formAuthor">
              <Form.Label>Author</Form.Label>
              <Form.Control
                type="text"
                ref={authorRef}
                defaultValue={item.author}
                required
              />
            </Form.Group>

            <Form.Group className="mb-3" controlId="formPages">
              <Form.Label>Number of Pages</Form.Label>
              <Form.Control
                type="number"
                ref={pagesRef}
                defaultValue={item.pages}
                min="1"
              />
            </Form.Group>

            <Form.Group className="mb-3" controlId="formRunTimeMinutes">
              <Form.Label>Run Time (Minutes)</Form.Label>
              <Form.Control
                type="number"
                ref={runTimeMinutesRef}
                defaultValue={item.runTimeMinutes}
                min="1"
              />
            </Form.Group>

            <Form.Group className="mb-3" controlId="formCategory">
              <Form.Label>Category</Form.Label>
              <Form.Control
                type="text"
                ref={categoryRef}
                defaultValue={item.Category?.CategoryName}
              />
            </Form.Group>

            <Form.Group className="mb-3" controlId="formBorrower">
              <Form.Label>Borrower</Form.Label>
              <Form.Control
                type="text"
                ref={borrowerRef}
                defaultValue={item.borrower}
              />
            </Form.Group>

            <Form.Group className="mb-3" controlId="formType">
        <Form.Label>Type</Form.Label>
        <Form.Control
          as="select"
          ref={typeRef}
          defaultValue={item.type}
          required
        >
          <option value="">Select a type</option>
          {Object.keys(LibraryItemType).map((key) => (
            <option key={key} value={LibraryItemType[key]}>
              {key}
            </option>
          ))}
        </Form.Control>
      </Form.Group>
            <Button
              variant="primary"
              type="button"
              onClick={updateLibraryItemHandler}
            >
              Submit
            </Button>
          </>
        )}
      </Form>
    </>
  );
}

export default UpdateLibraryItem;
