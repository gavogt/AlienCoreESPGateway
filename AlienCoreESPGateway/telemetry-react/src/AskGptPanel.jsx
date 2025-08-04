import React, { useState } from 'react';
import {
    Card,
    CardContent,
    Typography,
    TextField,
    Button,
    Box
} from '@mui/material';

export default function AskGptPanel({ onAsk }) {
    const [question, setQuestion] = useState('');
    const [answer, setAnswer] = useState('');

    const handleAsk = async () => {
        if (!question.trim()) return;
        const resp = await onAsk(question);
        setAnswer(resp);
    };

    return (
        <Card sx={{ mb: 4 }}>
            <CardContent>
                <Typography variant="h6" gutterBottom>
                    Ask GPT about this data
                </Typography>

                <TextField
                    label="Your question"
                    placeholder="What do you want to know?"
                    fullWidth
                    multiline
                    minRows={2}
                    value={question}
                    onChange={e => setQuestion(e.target.value)}
                    sx={{ mb: 2 }}
                />

                <Box sx={{ display: 'flex', gap: 1 }}>
                    <Button
                        variant="contained"
                        color="primary"
                        onClick={handleAsk}
                    >
                        Ask GPT
                    </Button>
                    <Button
                        variant="outlined"
                        onClick={() => {
                            setQuestion('');
                            setAnswer('');
                        }}
                    >
                        Clear
                    </Button>
                </Box>

                {answer && (
                    <Card sx={{ mt: 3, bgcolor: '#f9f9f9' }}>
                        <CardContent>
                            <Typography variant="subtitle2" gutterBottom>
                                GPT Answer
                            </Typography>
                            <Typography variant="body2">{answer}</Typography>
                        </CardContent>
                    </Card>
                )}
            </CardContent>
        </Card>
    );
}