#include <opencv2/opencv.hpp>
#include <opencv2/ml.hpp>
#include <iostream>
#include <filesystem>
#include <vector>
#include <random>

using namespace cv;
using namespace cv::ml;
using namespace std;
namespace fs = std::filesystem;


Mat extractFeatures(const Mat& image) {
    Mat resizedImage;
    resize(image, resizedImage, Size(700, 700));
    Mat grayImage;
    cvtColor(resizedImage, grayImage, COLOR_BGR2GRAY); 
    Mat featureVector;
    grayImage.reshape(1, 1).convertTo(featureVector, CV_32F); 
    return featureVector;
}


Mat skinDetection(const Mat& image) {
    Mat ycrcb;
    cvtColor(image, ycrcb, COLOR_BGR2YCrCb); 
    vector<Mat> channels;
    split(ycrcb, channels);

    Mat cr = channels[1];
    Mat cb = channels[2];

    Mat skin;
    skin = Mat::zeros(cr.size(), CV_8UC1); 

    
    for (int i = 0; i < skin.rows; i++) {
        for (int j = 0; j < skin.cols; j++) {
            if ((cr.at<uchar>(i, j) >= 135 && cr.at<uchar>(i, j) <= 200) && (cb.at<uchar>(i, j) >= 80 && cb.at<uchar>(i, j) <= 126)) {
                skin.at<uchar>(i, j) = 255; 
            }
        }
    }

    
    Mat denoisedSkin;
    morphologyEx(skin, denoisedSkin, MORPH_OPEN, getStructuringElement(MORPH_ELLIPSE, Size(5, 5)));


    vector<vector<Point>> contours;
    findContours(denoisedSkin, contours, RETR_EXTERNAL, CHAIN_APPROX_SIMPLE);

  
    Mat result = Mat::zeros(image.size(), CV_8UC3);


    drawContours(result, contours, -1, Scalar(255, 255, 255), FILLED);

    return result;
}

int main() {
  
    Ptr<RTrees> model = RTrees::create();

    
    Mat trainingData; 
    Mat labels; 

    string dataPath = "/home/parallels/Desktop/opencv_project/3/exp"; 

 
    random_device rd;
    mt19937 gen(rd());
    uniform_int_distribution<int> rotDist(-10, 10); 
    uniform_int_distribution<int> transDist(-10, 10); 
    uniform_real_distribution<double> scaleDist(0.9, 1.1); 

    for (const auto& entry : fs::directory_iterator(dataPath)) {
        if (entry.is_directory()) {
            string folderPath = entry.path().string();
            int count = 0;
            for (const auto& file : fs::directory_iterator(folderPath)) {
                if (file.is_regular_file()) {
                    string imagePath = file.path().string();
                    Mat image = imread(imagePath);
                    if (!image.empty()) {
                        for (int i = 0; i < 5; i++) { 
                           
                            double angle = rotDist(gen);
                            Mat rotatedImage;
                            Point2f center(image.cols / 2.0, image.rows / 2.0);
                            Mat rotMat = getRotationMatrix2D(center, angle, 1.0);
                            warpAffine(image, rotatedImage, rotMat, image.size());

                          
                            int tx = transDist(gen);
                            int ty = transDist(gen);
                            Mat translatedImage;
                            Mat transMat = (Mat_<double>(2, 3) << 1, 0, tx, 0, 1, ty);
                            warpAffine(rotatedImage, translatedImage, transMat, image.size());

                            
                            double scale = scaleDist(gen);
                            Mat scaledImage;
                            resize(translatedImage, scaledImage, Size(), scale, scale);

                           
                            Mat skinMask = skinDetection(scaledImage);

                           
                            Mat featureVector = extractFeatures(skinMask);

                            trainingData.push_back(featureVector);
                            labels.push_back(stoi(entry.path().filename()));
                            count++;
                        }
                    }
                }
                if (count >= 70) 
                    break;
            }
        }
    }

  
    model->train(trainingData, ROW_SAMPLE, labels);
    model->save("random_forest_model.xml");

   
    Mat predictions;
    model->predict(trainingData, predictions);

   
    int correctCount = 0;
    int totalCount = trainingData.rows;
    for (int i = 0; i < totalCount; i++) {
        float predictedLabel = predictions.at<float>(i, 0);
        float trueLabel = labels.at<int>(i, 0);
        if (predictedLabel == trueLabel)
            correctCount++;
    }

    float accuracy = static_cast<float>(correctCount) / totalCount;
    cout <<  << accuracy * 100 << "%" << endl;
    
    string testFolderPath = "/home/parallels/Desktop/opencv_project/3/Test set for gestures 0 to 9-20230531"; 
    for (const auto& entry : fs::directory_iterator(testFolderPath)) {
        if (entry.is_regular_file()) {
            string imagePath = entry.path().string();
            Mat testImage = imread(imagePath);
            if (!testImage.empty()) {
                Mat testSkinMask = skinDetection(testImage);
                Mat testFeatureVector = extractFeatures(testSkinMask);

            
                float predictedLabel = model->predict(testFeatureVector);

                cout << "image：" << imagePath << "，number: " << predictedLabel << endl;
            }
        }
    }

    return 0;
}
