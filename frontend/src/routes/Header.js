import { useNavigate } from "react-router-dom"

export default function Header() {
    const navigate = useNavigate();
    return (
        <div className="header">
            <div className="wrapper">                
                <button className="header_logo" onClick={() => navigate("/")} ><h1>Объекты</h1></button>
                {
                    !localStorage.getItem('token')
                        ? <>
                            <button className="header_btn" onClick={() => navigate("/login")} >Войти</button>
                        </>
                        : <>
                            <button className="header_btn" onClick={() => navigate("/new")} >Создать объявление</button>
                            <button className="header_btn" onClick={() => navigate("/profile")} >{localStorage.getItem('email')}</button>
                        </>
                }
            </div>
        </div>
    )
}