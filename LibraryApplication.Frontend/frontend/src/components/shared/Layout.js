import Container from 'react-bootstrap/Container';
import Navbar from 'react-bootstrap/Navbar';


function Layout(props) {
    return (
    <>
   
         <Navbar bg="white" variant="dark">
            <Container>
            <h2 style={{color:"black"}}> Consid Library</h2>
            
            <Navbar.Brand href="/">
            
            <img          
              src="https://consid.se/wp-content/uploads/2020/02/consid.se_logo-2.svg"
              width="455"
              height="100"
              className="d-inline-block align-top"
              alt="logo"
            />{''}
             
          </Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        
       </Container>
        
    </Navbar>
    
    <Container>{props.children} 
    
    </Container>
  
  </>
    );
    


    
}

export default Layout; 