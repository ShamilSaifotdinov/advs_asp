import { useState } from "react"
import useQuery from "../hooks/hook.http"

export default function Login() {
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

    const login = async (event, form) => {
        console.log(form)
        event.preventDefault()
        const res = await http("/login", "POST", JSON.stringify(form))
        if (errorState) {            
            console.error(errorState.message)
            alert(errorState.message)
        } else {
            console.log(res.Message)
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