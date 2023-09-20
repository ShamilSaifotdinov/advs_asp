import { useEffect } from "react"
import { useNavigate } from "react-router-dom"
import ProfileAdvs from "./ProfileAdvs";

export default function Profile() {
    const navigate = useNavigate();
    useEffect(() => { 
        if (!localStorage.getItem('token')) {
            navigate("/")
        }}, [])

    /*
    // useEffect(() => {
    //     fetch(`http://${window.location.hostname}:8080/profile`,
    //         {
    //             method: "GET",
    //             headers: {
    //                 "Accept": "application/json",
    //                 "Authorization": "Bearer " + sessionStorage.getItem('token')  // передача токена в заголовке
    //             }
    //         })
    //         .then(res => res.json())
    //         .then(
    //             (result) => {
    //                 setLoading(false);
    //                 setAdvs(result);
    //             },
    //             // Note: it's important to handle errors here
    //             // instead of a catch() block so that we don't swallow
    //             // exceptions from actual bugs in components.
    //             (error) => {
    //                 setLoading(false);
    //                 setError(error);
    //             }
    //         )
    // }, [])
    */
    
    const logout = (event) => {        
        event.preventDefault()
        localStorage.clear()
        navigate("/");
    }

    return (
        <div className="Profile">
            <div className="wrapper">
                <header className="Profile_header">
                    <h1>{localStorage.getItem('email')}</h1>
                    <a className="header_btn" onClick={(event) => logout(event)}>Выйти</a>
                </header>
                {/* <button className="header_btn" onClick={logout}>Выйти</button> */}
                { <ProfileAdvs /> }
            </div>
        </div>

    )
}