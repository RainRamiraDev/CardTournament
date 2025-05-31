import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import LogInForm from './components/LogInForm';

function App() {
  const [count, setCount] = useState(0)

  return (
   <LogInForm />
  )
}

export default App
