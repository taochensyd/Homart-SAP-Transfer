import React, { useState } from 'react';
import axios from 'axios';

const LoginForm = () => {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [firstName, setFirstName] = useState('');

    const handleSubmit = async (event) => {
        event.preventDefault();
        const response = await axios.post('https://localhost:7134/api/v1/SapLoginController', {
            username,
            password,
        });
        if (response.data.success) {
            // set the SessionId to the global variable
            window.SessionId = response.data.SessionId;
            // display the "firstName" to the top of the page
            setFirstName(response.data.values.firstName);
        }
    };

    return (
        <>
            {firstName ? (
                <div>{firstName}</div>
            ) : (
                <form onSubmit={handleSubmit}>
                    <input
                        type="text"
                        placeholder="Username"
                        value={username}
                        onChange={(event) => setUsername(event.target.value)}
                    />
                    <input
                        type="password"
                        placeholder="Password"
                        value={password}
                        onChange={(event) => setPassword(event.target.value)}
                    />
                    <button type="submit">Login</button>
                </form>
            )}
        </>
    );
};

export default LoginForm;