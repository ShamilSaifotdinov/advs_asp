import { useEffect, useState } from "react"
import useQuery from "../hooks/hook.http"
import { useNavigate } from "react-router-dom"

export default function Login() {
    const navigate = useNavigate();
    const [ http, loading, errorState ] = useQuery();
    const [ form, setForm ] = useState({
        Email: "",
        Password: ""
    });

    const handleChange = event => {
        setForm({
            ...form,
            [event.target.name]: event.target.value
        })
    }
    
    useEffect(() => {
        if (errorState) {
            console.error(errorState)
            alert(errorState)
        }
    },[errorState])

    const login = async (event, form) => {
        console.log(form)
        event.preventDefault()
        const res = await http("/login", "POST", JSON.stringify(form), {"Content-Type": "application/json"})
        if (res) {
            console.log(res);
            for (const key in res) {                
                localStorage.setItem(key, res[key]);
                console.log(`${key}: ${res[key]}`);
            }            
            navigate("/profile");
        }
    }

    return (
        <div className="login">
            {                
                loading 
                ? <h1>Loading...</h1>
                : 
                    <div className="login-component">
                        <h1 className="login-component_header">Аутентификация</h1>
                        <form className="login-component_form login-form" onSubmit={(event) => login(event, form)}>
                            <label className="login-form_item" htmlFor="E-mail">E-mail</label>
                            <input name="Email" id="E-mail" onChange={handleChange} value={form.Email} required />
                            <label className="login-form_item" htmlFor="Password">Password</label>
                            <input name="Password" id="Password" type="password" onChange={handleChange} value={form.Password} required />
                            <input className="login-form_item login-btn" type="submit" value="Войти" />
                        </form>
                    </div>
            }
        </div>
    )
}