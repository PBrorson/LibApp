import { useEffect, useState } from "react";
import axios from "axios";
import Card from "react-bootstrap/Card";
import Col from "react-bootstrap/Col";
import Row from "react-bootstrap/Row";
import Button from "react-bootstrap/Button";
import { useNavigate } from "react-router-dom";
import Modal from "react-bootstrap/Modal";
import Form from "react-bootstrap/Form";
import DeleteConfirmation from "../components/shared/DeleteConfirmation";
import { LibraryItemType } from "./LibraryItemType";

function AllLibraryItems() {
  const [libraryItems, setLibraryItems] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [itemToDelete, setItemToDelete] = useState(0);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [categoryName, setCategoryName] = useState("");
  const [categoryEditId, setCategoryEditId] = useState(null);
  const [selectedCategoryId, setSelectedCategoryId] = useState(null);
  const [categories, setCategories] = useState([]);
  const navigate = useNavigate();

  function showConfirmPopUpHandler(id) {
    setShowModal(true);
    setItemToDelete(id);
  }
  function handleCheckOut(itemId) {
    const borrower = prompt("Enter the borrower name:");

    if (borrower) {
      axios
        .post(
          `https://localhost:7282/api/LibraryItems/items/${itemId}/checkout`,
          `"${borrower}"`,
          {
            headers: {
              "Content-Type": "application/json",
            },
          }
        )
        .then((response) => {
          const updatedItems = libraryItems.map((item) => {
            if (item.id === itemId) {
              return {
                ...item,
                borrower: response.data.borrower,
                isBorrowable: true,
              };
            }
            return item;
          });
          setLibraryItems(updatedItems);
          console.log("Item checked out successfully:", response.data);
        })
        .catch((error) => {
          console.error("Error checking out item:", error);
        });
    }
  }
  function closeConfirmPopUpHandler() {
    setShowModal(false);
    setItemToDelete(0);
  }
  function handleEditCategory(categoryId) {
    if (categoryId) {
      editCategoryHandler(categoryId);
    } else {
      console.log(categoryId);
    }
  }
  function createCategoryHandler() {
    setShowCreateModal(true);
  }

  function editCategoryHandler(categoryId) {
    setCategoryEditId(categoryId);
    setShowEditModal(true);

    const selectedCategory = categories.find(
      (category) => category.id === categoryId
    );

    if (selectedCategory) {
      setCategoryName(selectedCategory.categoryName);
    } else {
      setCategoryName("");
    }
  }

  function deleteConfirmHandler() {
    axios
      .delete(
        `https://localhost:7282/api/LibraryItems/DeleteItemById/${itemToDelete}`
      )
      .then((response) => {
        setLibraryItems((existingData) => {
          return existingData.filter((_) => _.id !== itemToDelete);
        });
        setItemToDelete(0);
        setShowModal(false);
      });
  }

  function createCategory() {
    const newCategory = { categoryName };

    axios
      .post("https://localhost:7282/api/Categories", newCategory)
      .then((response) => {
        setCategories([...categories, response.data]);
        closeCreateModal();
      })
      .catch((error) => {
        console.error(error);
      });
  }
  function deleteCategory() {
    if (selectedCategoryId) {
      axios
        .delete(`https://localhost:7282/api/Categories/${selectedCategoryId}`)
        .then((response) => {
          const isCategoryUsed = libraryItems.some(
            (item) => item.category.id === selectedCategoryId
          );

          if (isCategoryUsed) {
            alert(
              "Cannot delete the category. There are library items associated with it."
            );
            return;
          }

          console.log(response.data);
          closeEditModal();
          setCategories((prevCategories) =>
            prevCategories.filter(
              (category) => category.id !== selectedCategoryId
            )
          );
          setSelectedCategoryId(null);
        })
        .catch((error) => {
          console.error(error);
        });
    }
  }

  function editCategory(categoryId) {
    const updatedCategory = { id: categoryId, categoryName };
    console.log(updatedCategory);

    axios
      .put(`https://localhost:7282/Updatecategory/${categoryId}`, {
        categoryName: updatedCategory.categoryName,
      })
      .then((response) => {
        console.log(response.data);
        closeEditModal();

        setCategories((prevCategories) =>
          prevCategories.map((category) =>
            category.id === categoryId ? updatedCategory : category
          )
        );
      })
      .catch((error) => {
        console.error(error);
      });
  }

  function closeCreateModal() {
    setShowCreateModal(false);
    setCategoryName("");
  }

  function closeEditModal() {
    setShowEditModal(false);
    setCategoryName("");
    setSelectedCategoryId(null);
  }

  function handleCategoryChange(e) {
    const categoryId = e.target.value;
    setSelectedCategoryId(categoryId);
    const selectedCategory = categories.find(
      (category) => category.id === parseInt(categoryId)
    );
    if (selectedCategory) {
      console.log("Selected category:", selectedCategory);
      setCategoryName(selectedCategory.categoryName);
    } else {
      console.log("No category found for ID:", categoryId);
      setCategoryName("");
    }
  }
 
  useEffect(() => {
    axios.get("https://localhost:7282/GetAllItems").then((response) => {
      const sortedItems = response.data.sort((a, b) =>
        a.category.categoryName.localeCompare(b.category.categoryName)
      );
      setLibraryItems(sortedItems);
    });

    axios.get("https://localhost:7282/api/Categories").then((response) => {
      setCategories(response.data);
    });
  }, []);
  return (
    <>
      <DeleteConfirmation
        showModal={showModal}
        title="Delete Confirmation!"
        body="Are you sure you want to delete this library item?"
        closeConfirmPopUpHandler={closeConfirmPopUpHandler}
        deleteConfirmHandler={deleteConfirmHandler}
      />{" "}
      <Row className="mt-">
        <Col md={4}>
          <Button
            variant="success"
            type="button"
            onClick={() => navigate("/create")}
            className="me-2"
          >
            Add Library Item
          </Button>
        </Col>
        <Col md={4}>
          <Button
            variant="primary"
            type="button"
            onClick={createCategoryHandler}
            className="me-2"
          >
            Create Category
          </Button>
        </Col>
        <Col md={4}>
          <Button
            variant="primary"
            type="button"
            onClick={() => setShowEditModal(true)}
          >
            Edit Category
          </Button>
        </Col>
      </Row>
      <Row xs={1} md={3} className="g-4 mt-1">
        {libraryItems.map((item) => (
          <Col key={item.id}>
            <Card style={{ width: "21em" }}>
              <Card.Body>
                <Card.Title>
                  <h2>
                    {" "}
                    {item.title} ({item.titleAcronym})
                  </h2>
                </Card.Title>
                <Card.Text>
                  <b>Category:</b> {item.category.categoryName}
                </Card.Text>
                <Card.Text>
                  <b>Borrower:</b> {item.borrower}
                </Card.Text>
                <Card.Text>
                  <b>Author:</b> {item.author}
                </Card.Text>
                <Card.Text>
                  <b>Pages:</b> {item.pages}
                </Card.Text>
                <Card.Text>
                  <b>Type:</b>{" "}
                  {Object.keys(LibraryItemType).find(
                    (key) => LibraryItemType[key] === item.type
                  )}
                </Card.Text>
                <Button
                  variant="outline-dark"
                  type="button"
                  onClick={() => navigate(`/update-item/${item.id}`)}
                >
                  Edit Library Item
                </Button>
                <Button
                  variant="outline-danger"
                  type="button"
                  onClick={() => showConfirmPopUpHandler(item.id)}
                >
                  Delete Library Item
                </Button>
                <Button
                  variant="outline-danger"
                  type="button"
                  onClick={() => handleCheckOut(item.id)}
                >
                  Check out Item
                </Button>
              </Card.Body>
            </Card>
          </Col>
        ))}
      </Row>
      <Modal show={showCreateModal} onHide={closeCreateModal}>
        <Modal.Header closeButton>
          <Modal.Title>Create Category</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group controlId="categoryName">
              <Form.Label>Category Name</Form.Label>
              <Form.Control
                type="text"
                value={categoryName}
                onChange={(e) => setCategoryName(e.target.value)}
              />
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={closeCreateModal}>
            Close
          </Button>
          <Button variant="primary" onClick={createCategory}>
            Create
          </Button>
        </Modal.Footer>
      </Modal>
      <Modal show={showEditModal} onHide={closeEditModal}>
        <Modal.Header closeButton>
          <Modal.Title>Edit Category</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group controlId="categorySelect">
              <Form.Label>Select Category</Form.Label>
              <Form.Control
                as="select"
                value={selectedCategoryId || ""}
                onChange={handleCategoryChange}
              >
                <option value="">Select a category</option>
                {categories.map((category) => (
                  <option key={category.id} value={category.id}>
                    {category.categoryName}
                  </option>
                ))}
              </Form.Control>
            </Form.Group>
            {selectedCategoryId && (
              <Form.Group controlId="categoryName">
                <Form.Label>New Category Name</Form.Label>
                <Form.Control
                  type="text"
                  value={categoryName}
                  onChange={(e) => setCategoryName(e.target.value)}
                />
              </Form.Group>
            )}
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="danger" onClick={deleteCategory}>
            Delete Category
          </Button>
          <Button variant="secondary" onClick={closeEditModal}>
            Close
          </Button>
          <Button
            variant="primary"
            onClick={() => editCategory(selectedCategoryId)}
          >
            Save Changes
          </Button>
        </Modal.Footer>
      </Modal>
    </>
  );
}

export default AllLibraryItems;
