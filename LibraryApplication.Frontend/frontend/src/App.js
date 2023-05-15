import './App.css';
import Layout from './components/shared/Layout';
import AllLibraryItems from './components/AllLibraryItems';
import { Route, Routes } from 'react-router-dom';
import AddLibraryItem from './components/AddLibraryItem';
import UpdateLibraryItem from './components/UpdateLibraryItem';

function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<AllLibraryItems />} />
        <Route path="create" element={<AddLibraryItem />} />
        <Route path="/update-item/:id" element={<UpdateLibraryItem />} />
      </Routes>
    </Layout>
  );
}

export default App;
