import { useState } from "react"
import useQuery from "../hooks/hook.http"
import { useNavigate } from "react-router-dom"



export default function NewAdv() {
    const navigate = useNavigate();
    const [ http, loading, errorState ] = useQuery();
    const [ form, setForm ] = useState({
        Name: "",
        Location: "",
        Discription: "",
        Price: 0,
        UserId: 1
    });

    const sendNewAdv = async (event, form) => {
        console.log(form)
        event.preventDefault()
        const res = await http("/advs/new", "POST", JSON.stringify(form))
        if (errorState) {            
            console.error(errorState.message)
            alert(errorState.message)
        } else {
            console.log(res)
            navigate("/objects/" + res.ID)
        }
    }

    const handleChange = event => {
        setForm({
            ...form,
            [event.target.name]: event.target.value
        })
    }

    const handleChangeInt = event => {
        if (event.nativeEvent.data >= '0' && event.nativeEvent.data <= '9') {
            setForm({
                ...form,
                [event.target.name]: Number(event.target.value)
            })
        }
    }

    return (
        loading 
        ? <h1>Loading...</h1>
        : <div className="NewAdv">
            <div className="wrapper">
                <h1>Новое объявление</h1>
                <form onSubmit={(event) => sendNewAdv(event, form)}>
                    <div className="field">
                        <label className="field_label" htmlFor="name">Наименование*</label>
                        <input className="field_input" name="Name" id="name" onChange={handleChange} value={form.Name} required />
                    </div>
                    <div className="field">
                        <label className="field_label" htmlFor="location">Местоположение*</label>
                        <input className="field_input" name="Location" id="location" onChange={handleChange} required />
                    </div>
                    <div className="field">
                        <label className="field_label" htmlFor="Discription">Описание</label>
                        <textarea className="field_input NewAdv-disc" name="Discription" id="Discription" onChange={handleChange} />
                    </div>
                    <div className="field">
                        <label className="field_label" htmlFor="price">Цена</label>
                        <input className="field_input" name="Price" id="price" value={form.Price} onChange={handleChangeInt} pattern="[0-9]+" />
                    </div>
                    <input type="submit" value="Создать" />
                </form>
            </div>
        </div>
    )
}