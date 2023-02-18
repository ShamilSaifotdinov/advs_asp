export default function Header() {
    return (
        <div className="header">
            <div className="wrapper">                
                <a href="/"><h1>Объекты</h1></a>
                <a className="header_btn" href="/new">Создать объявление</a>
                <a className="header_btn" href="/login">Войти</a>
            </div>
        </div>
    )
}